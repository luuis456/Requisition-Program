// Mobile fulfillment interactions: quantities, shortage reasons, review toggles, finalize.
(function () {
    'use strict';

    var context = window.fulfillmentContext || {};
    var reviewedItems = context.reviewedItems || 0;
    var totalItems = context.totalItems || 0;

    function parseNumber(value) {
        var parsed = parseFloat(String(value).replace(',', '.'));
        return isNaN(parsed) ? 0 : parsed;
    }

    function cardOf(element) {
        return element.closest('.material-card');
    }

    function getQuantity(card) {
        return parseNumber(card.querySelector('[data-quantity-input]').value);
    }

    function getRequested(card) {
        return parseFloat(card.getAttribute('data-requested'));
    }

    function getReasonId(card) {
        var select = card.querySelector('[data-reason-select]');
        return select.value ? parseInt(select.value, 10) : null;
    }

    function getComment(card) {
        return (card.querySelector('[data-comment-input]') || { value: '' }).value.trim();
    }

    function showError(card, message) {
        var errorElement = card.querySelector('[data-error]');
        if (!errorElement) { return; }
        if (message) {
            errorElement.textContent = message;
            errorElement.classList.remove('d-none');
        } else {
            errorElement.textContent = '';
            errorElement.classList.add('d-none');
        }
    }

    function updateStatusBadge(card, label, tone) {
        var badge = card.querySelector('[data-status-badge]');
        badge.className = 'fulfillment-badge fulfillment-' + tone;
        badge.setAttribute('data-status-badge', '');
        badge.textContent = label;
    }

    function setReviewedVisual(card, reviewed) {
        card.classList.toggle('reviewed', reviewed);
        var toggle = card.querySelector('[data-review-toggle]');
        toggle.className = 'btn review-toggle w-100 mt-3 ' + (reviewed ? 'btn-outline-success' : 'btn-success');
        toggle.querySelector('[data-review-icon]').className =
            'bi ' + (reviewed ? 'bi-pencil' : 'bi-check2-circle') + ' me-1';
        toggle.querySelector('[data-review-label]').textContent =
            reviewed ? 'Editar material' : 'Marcar como revisado';
    }

    function updateProgress() {
        var label = document.getElementById('progressLabel');
        var bar = document.getElementById('fulfillmentProgressBar');
        if (!label || !bar || !totalItems) { return; }
        label.textContent = reviewedItems + ' de ' + totalItems + ' materiales revisados';
        bar.style.width = Math.round(reviewedItems * 100 / totalItems) + '%';
    }

    function sendUpdate(card, reviewed, done) {
        var payload = {
            RequisitionId: context.requisitionId,
            ItemId: parseInt(card.getAttribute('data-item-id'), 10),
            FulfilledQuantity: getQuantity(card),
            ShortageReasonId: getReasonId(card),
            ShortageComment: getComment(card),
            Reviewed: reviewed
        };

        fetch('/Fulfillment/UpdateItem', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': window.App.getAntiForgeryToken()
            },
            body: JSON.stringify(payload)
        })
            .then(function (response) {
                return response.json().then(function (data) { return { ok: response.ok, data: data }; });
            })
            .then(function (result) {
                if (!result.ok || !result.data.success) {
                    throw new Error(result.data.message || 'No fue posible guardar los cambios.');
                }
                var data = result.data;
                reviewedItems = data.progressReviewed;
                totalItems = data.progressTotal;
                updateProgress();
                updateStatusBadge(card, data.fulfillmentStatusLabel, data.fulfillmentStatusTone);
                setReviewedVisual(card, data.reviewed);
                showError(card, null);
                if (done) { done(data); }
            })
            .catch(function (error) {
                showError(card, error.message);
                window.App.toast(error.message, 'error');
            });
    }

    // Quantity steppers.
    document.addEventListener('click', function (event) {
        var stepper = event.target.closest('[data-step]');
        if (!stepper) { return; }

        var card = cardOf(stepper);
        var input = card.querySelector('[data-quantity-input]');
        var requested = getRequested(card);
        var current = getQuantity(input);
        var stepValue = parseFloat(stepper.getAttribute('data-step'));

        if (card.classList.contains('reviewed')) { reopenCard(card); }

        input.value = Math.min(requested, Math.max(0, current + stepValue));
        validateLocally(card);
        input.focus();
    });

    function reopenCard(card) {
        card.classList.remove('reviewed');
        setReviewedVisual(card, false);
        if (reviewedItems > 0) {
            reviewedItems -= 1;
            updateProgress();
        }
    }

    function validateLocally(card) {
        var quantity = getQuantity(card);
        var requested = getRequested(card);

        if (quantity < 0) {
            showError(card, 'La cantidad no puede ser negativa.');
            return false;
        }
        if (quantity > requested) {
            showError(card, 'La cantidad surtida no puede ser mayor que la cantidad solicitada.');
            return false;
        }
        if (quantity < requested && !getReasonId(card)) {
            showError(card, 'Si hay faltante debes seleccionar una razón.');
            return false;
        }

        showError(card, null);
        return true;
    }

    // Show the comment box when the selected reason requires it.
    document.addEventListener('change', function (event) {
        var reasonSelect = event.target.closest('[data-reason-select]');
        if (!reasonSelect) { return; }

        var card = cardOf(reasonSelect);
        var option = reasonSelect.selectedOptions[0];
        var requiresComment = option && option.getAttribute('data-requires-comment') === 'True';
        card.querySelector('[data-comment-wrap]').classList.toggle('d-none', !requiresComment);

        if (card.classList.contains('reviewed')) {
            reopenCard(card);
        }
        validateLocally(card);
    });

    document.addEventListener('input', function (event) {
        var quantityInput = event.target.closest('[data-quantity-input]');
        if (!quantityInput) { return; }

        var card = cardOf(quantityInput);
        if (card.classList.contains('reviewed')) {
            reopenCard(card);
        }
        validateLocally(card);
    });

    document.addEventListener('input', function (event) {
        var commentInput = event.target.closest('[data-comment-input]');
        if (commentInput && cardOf(commentInput).classList.contains('reviewed')) {
            reopenCard(cardOf(commentInput));
        }
    });

    // Review toggle.
    document.addEventListener('click', function (event) {
        var toggle = event.target.closest('[data-review-toggle]');
        if (!toggle) { return; }

        var card = cardOf(toggle);

        if (card.classList.contains('reviewed')) {
            // Allow editing a previously reviewed material.
            setReviewedVisual(card, false);
            card.classList.remove('reviewed');
            window.App.toast('Material habilitado para edición. Vuelve a marcarlo como revisado al terminar.', 'info');
            return;
        }

        if (!validateLocally(card)) { return; }
        sendUpdate(card, true, function () {
            window.App.toast('Material revisado.', 'success');
        });
    });

    // Finalize with confirmation.
    var finalizeForm = document.getElementById('finalizeForm');
    if (finalizeForm) {
        finalizeForm.addEventListener('submit', function (event) {
            event.preventDefault();

            var unreviewed = totalItems - reviewedItems;
            if (unreviewed > 0) {
                window.App.toast(
                    'Debes revisar todos los materiales antes de finalizar. Faltan ' + unreviewed + '.',
                    'warning');
                return;
            }

            window.App.confirm(
                'Finalizar requisición',
                '¿Estás seguro de que deseas finalizar esta requisición? Una vez finalizada se moverá al historial y dejará de aparecer en pendientes.',
                { confirmText: 'Sí, finalizar', tone: 'primary' }
            ).then(function (confirmed) {
                if (confirmed) {
                    // form.submit() skips validation and this handler entirely.
                    finalizeForm.submit();
                }
            });
        });
    }

    updateProgress();
})();
