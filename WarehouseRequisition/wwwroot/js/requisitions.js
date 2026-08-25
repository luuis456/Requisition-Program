// Pending list + history + create-requisition interactions.
(function () {
    'use strict';

    // ------------------------------------------------------------------
    // Requisition delete (pending list)
    // ------------------------------------------------------------------
    document.addEventListener('click', function (event) {
        var button = event.target.closest('[data-delete-requisition]');
        if (!button) { return; }

        event.preventDefault();
        var id = button.getAttribute('data-delete-requisition');
        var number = button.getAttribute('data-number');

        window.App.confirm(
            'Eliminar requisición',
            '¿Deseas eliminar la requisición ' + number + '? Esta acción no se puede deshacer.',
            { confirmText: 'Sí, eliminar' }
        ).then(function (confirmed) {
            if (!confirmed) { return; }

            var token = window.App.getAntiForgeryToken();
            var body = new URLSearchParams();
            body.append('id', id);
            body.append('__RequestVerificationToken', token);

            fetch('/Requisitions/Delete', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8' },
                body: body.toString()
            }).then(function () {
                window.location.reload();
            });
        });
    });

    // ------------------------------------------------------------------
    // Create requisition
    // ------------------------------------------------------------------
    var form = document.getElementById('requisitionForm');
    if (!form) { return; }

    var items = Array.isArray(window.initialItems) ? window.initialItems.slice() : [];
    var editingIndex = null;
    var tbody = document.getElementById('materialsBody');
    var emptyState = document.getElementById('materialsEmptyState');
    var inputsContainer = document.getElementById('itemsInputsContainer');
    var countBadge = document.getElementById('itemsCountBadge');

    function escapeHtml(value) {
        return String(value == null ? '' : value)
            .replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;').replace(/'/g, '&#39;');
    }

    function render() {
        if (!tbody) { return; }
        tbody.innerHTML = items.map(function (item, index) {
            return '<tr data-label-row>' +
                '<td data-label="Número de parte" class="fw-semibold">' + escapeHtml(item.partNumber) + '</td>' +
                '<td data-label="Descripción">' + escapeHtml(item.description || '-') + '</td>' +
                '<td data-label="Cantidad solicitada" class="text-center fw-semibold">' + item.requestedQuantity + '</td>' +
                '<td data-label="Descripción de cantidad">' + escapeHtml(item.quantityDescription || '-') + '</td>' +
                '<td data-label="Unidad" class="text-center"><span class="items-chip">' + escapeHtml(item.unitOfMeasure || 'PZA') + '</span></td>' +
                '<td data-label="Ubicación"><span class="location-chip"><i class="bi bi-geo-alt me-1"></i>' + escapeHtml(item.location || '-') + '</span></td>' +
                '<td data-label="Observaciones" class="small text-secondary">' + escapeHtml(item.observations || '-') + '</td>' +
                '<td data-label="Acciones"><div class="d-inline-flex gap-1 justify-content-end action-buttons">' +
                '<button type="button" class="btn btn-icon btn-soft-primary" data-edit-item="' + index + '" title="Editar" aria-label="Editar"><i class="bi bi-pencil"></i></button>' +
                '<button type="button" class="btn btn-icon btn-soft-danger" data-remove-item="' + index + '" title="Eliminar" aria-label="Eliminar"><i class="bi bi-trash"></i></button>' +
                '</div></td></tr>';
        }).join('');

        if (emptyState) { emptyState.classList.toggle('d-none', items.length > 0); }
        if (countBadge) { countBadge.textContent = items.length; }
        syncHiddenInputs();
    }

    function syncHiddenInputs() {
        if (!inputsContainer) { return; }
        inputsContainer.innerHTML = items.map(function (item, index) {
            return hiddenInput('Items[' + index + '].PartNumber', item.partNumber) +
                hiddenInput('Items[' + index + '].Description', item.description) +
                hiddenInput('Items[' + index + '].RequestedQuantity', item.requestedQuantity) +
                hiddenInput('Items[' + index + '].QuantityDescription', item.quantityDescription) +
                hiddenInput('Items[' + index + '].UnitOfMeasure', item.unitOfMeasure) +
                hiddenInput('Items[' + index + '].Location', item.location) +
                hiddenInput('Items[' + index + '].Observations', item.observations);
        }).join('');
    }

    function hiddenInput(name, value) {
        return '<input type="hidden" name="' + name + '" value="' + escapeHtml(value) + '" />';
    }

    document.addEventListener('click', function (event) {
        var editTrigger = event.target.closest('[data-edit-item]');
        if (editTrigger && form.contains(editTrigger)) {
            openManualModal(parseInt(editTrigger.getAttribute('data-edit-item'), 10));
            return;
        }

        var removeTrigger = event.target.closest('[data-remove-item]');
        if (removeTrigger && form.contains(removeTrigger)) {
            var index = parseInt(removeTrigger.getAttribute('data-remove-item'), 10);
            window.App.confirm('Eliminar material', '¿Deseas eliminar este material?', { confirmText: 'Sí, eliminar' })
                .then(function (confirmed) {
                    if (confirmed) {
                        items.splice(index, 1);
                        render();
                    }
                });
        }
    });

    // ---------------- Manual part modal ----------------
    var manualModalElement = document.getElementById('manualPartModal');
    var manualModal = manualModalElement ? bootstrap.Modal.getOrCreateInstance(manualModalElement) : null;
    var partNumberInput = document.getElementById('partNumberInput');
    var quantityInput = document.getElementById('quantityInput');
    var quantityDescriptionInput = document.getElementById('quantityDescriptionInput');
    var observationsInput = document.getElementById('observationsInput');
    var infoPanel = document.getElementById('partInfoPanel');

    document.querySelectorAll('[data-bs-target="#manualPartModal"]').forEach(function (button) {
        button.addEventListener('click', function () {
            setTimeout(function () { resetManualModal(null); }, 150);
        });
    });

    function resetManualModal(index) {
        editingIndex = index;
        var existing = index !== null ? items[index] : null;
        partNumberInput.value = existing ? existing.partNumber : '';
        quantityInput.value = existing ? existing.requestedQuantity : '';
        quantityDescriptionInput.value = existing ? (existing.quantityDescription || '') : '';
        observationsInput.value = existing ? (existing.observations || '') : '';
        clearPartInfo();

        if (existing) {
            applyPartInfo({
                description: existing.description,
                unitOfMeasure: existing.unitOfMeasure,
                defaultLocation: existing.location
            }, false);
        }
        if (index === null) { partNumberInput.focus(); }
    }

    function clearPartInfo() {
        infoPanel.classList.add('d-none');
        document.getElementById('infoDescription').textContent = '-';
        document.getElementById('infoUnitOfMeasure').textContent = '-';
        document.getElementById('infoLocation').textContent = '-';
        setFeedback('');
    }

    function setFeedback(message, isError) {
        var feedback = document.getElementById('partLookupFeedback');
        feedback.textContent = message;
        feedback.className = 'form-text ' + (isError ? 'text-danger' : 'text-success');
    }

    function applyPartInfo(part, announce) {
        infoPanel.classList.remove('d-none');
        document.getElementById('infoDescription').textContent = part.description || '-';
        document.getElementById('infoUnitOfMeasure').textContent = part.unitOfMeasure || '-';
        document.getElementById('infoLocation').textContent = part.defaultLocation || '-';
        if (announce) { setFeedback('Parte encontrada en el catálogo.', false); }
    }

    var lookupTimer = null;
    partNumberInput.addEventListener('input', function () {
        clearTimeout(lookupTimer);
        var term = partNumberInput.value.trim();
        if (!term) { clearPartInfo(); hideSuggestions(); return; }
        lookupTimer = setTimeout(function () { lookupPart(term); }, 280);
    });

    function lookupPart(term) {
        fetch('/Catalog/SearchParts?term=' + encodeURIComponent(term))
            .then(function (response) { return response.json(); })
            .then(function (data) {
                if (data.found) {
                    applyPartInfo(data, true);
                    hideSuggestions();
                    showSuggestions([]);
                } else {
                    clearPartInfo();
                    showSuggestions(data.suggestions || []);
                }
            })
            .catch(function () { /* offline-safe */ });
    }

    var suggestionsElement = document.getElementById('partSuggestions');

    function showSuggestions(suggestions) {
        if (!suggestions.length) { hideSuggestions(); return; }
        suggestionsElement.innerHTML = suggestions.map(function (part) {
            return '<button type="button" class="list-group-item list-group-item-action py-2" ' +
                'data-suggestion="' + escapeHtml(part.partNumber) + '">' +
                '<strong>' + escapeHtml(part.partNumber) + '</strong> · ' + escapeHtml(part.description) + '</button>';
        }).join('');
        suggestionsElement.classList.remove('d-none');
    }

    function hideSuggestions() {
        suggestionsElement.classList.add('d-none');
        suggestionsElement.innerHTML = '';
    }

    suggestionsElement.addEventListener('click', function (event) {
        var option = event.target.closest('[data-suggestion]');
        if (!option) { return; }
        partNumberInput.value = option.getAttribute('data-suggestion');
        hideSuggestions();
        lookupPart(option.getAttribute('data-suggestion'));
    });

    document.addEventListener('click', function (event) {
        if (!event.target.closest('#partSuggestions') && event.target !== partNumberInput) {
            hideSuggestions();
        }
    });

    document.getElementById('saveMaterialButton').addEventListener('click', function () {
        var partNumber = partNumberInput.value.trim().toUpperCase();
        var quantity = parseFloat(String(quantityInput.value).replace(',', '.'));

        if (!partNumber) {
            setFeedback('El número de parte es obligatorio.', true);
            partNumberInput.focus();
            return;
        }
        if (isNaN(quantity) || quantity <= 0) {
            setFeedback('La cantidad debe ser mayor que cero.', true);
            quantityInput.focus();
            return;
        }

        var item = {
            partNumber: partNumber,
            description: document.getElementById('infoDescription').textContent.trim(),
            requestedQuantity: quantity,
            quantityDescription: quantityDescriptionInput.value.trim(),
            unitOfMeasure: document.getElementById('infoUnitOfMeasure').textContent.trim() || 'PZA',
            location: document.getElementById('infoLocation').textContent.trim(),
            observations: observationsInput.value.trim()
        };

        if (editingIndex === null) {
            items.push(item);
            window.App.toast('Material agregado a la requisición.', 'success');
        } else {
            items[editingIndex] = item;
            window.App.toast('Material actualizado.', 'success');
        }

        render();
        manualModal.hide();
    });

    // ---------------- Automatic generation modal ----------------
    var generateButton = document.getElementById('generateMaterialsButton');
    generateButton.addEventListener('click', function () {
        var orderNumber = document.getElementById('orderNumberInput').value.trim();
        var line = document.getElementById('lineSelect').value;
        var quantity = parseInt(document.getElementById('generateCountInput').value, 10);

        if (!orderNumber) {
            window.App.toast('Escribe el número de orden para generar los materiales.', 'error');
            return;
        }

        generateButton.disabled = true;
        generateButton.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Generando...';

        var payload = { OrderNumber: orderNumber, Line: line, Quantity: isNaN(quantity) ? 5 : quantity };

        fetch('/Requisitions/AutoGenerateMaterials', {
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
                    throw new Error(result.data.message || 'No fue posible generar los materiales.');
                }
                result.data.items.forEach(function (generated) { items.push(generated); });
                render();
                bootstrap.Modal.getInstance(document.getElementById('autoGenerateModal')).hide();
                window.App.toast(result.data.items.length + ' material(es) generados desde la orden ' + orderNumber + '.', 'success');
            })
            .catch(function (error) {
                window.App.toast(error.message, 'error');
            })
            .finally(function () {
                generateButton.disabled = false;
                generateButton.innerHTML = '<i class="bi bi-stars me-1"></i> Generar';
            });
    });

    // ---------------- Submit guard ----------------
    form.addEventListener('submit', function (event) {
        if (items.length === 0) {
            event.preventDefault();
            window.App.toast('Debes agregar al menos un material.', 'error');
            return;
        }

        var invalid = items.some(function (item) {
            return !item.partNumber || !(parseFloat(item.requestedQuantity) > 0);
        });
        if (invalid) {
            event.preventDefault();
            window.App.toast('Verifica que cada material tenga número de parte y cantidad válidos.', 'error');
        }
    });

    render();
})();
