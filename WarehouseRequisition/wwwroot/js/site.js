// Global helpers shared by every screen: toasts, confirmation dialog, details modal.
(function () {
    'use strict';

    function showToast(message, type) {
        var container = document.getElementById('toastContainer');
        if (!container) { return; }

        var icons = {
            success: 'bi-check-circle-fill',
            error: 'bi-x-circle-fill',
            warning: 'bi-exclamation-triangle-fill'
        };

        var element = document.createElement('div');
        element.className = 'toast align-items-center app-toast toast-' + (type || 'info') + ' border-0 show';
        element.setAttribute('role', 'status');
        element.innerHTML =
            '<div class="d-flex">' +
            '<div class="toast-body"><i class="bi ' + (icons[type] || 'bi-info-circle-fill') + '"></i><span></span></div>' +
            '<button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Cerrar"></button>' +
            '</div>';
        element.querySelector('span').textContent = message;
        container.appendChild(element);

        var toast = new bootstrap.Toast(element, { delay: 4200 });
        toast.show();
        element.addEventListener('hidden.bs.toast', function () { element.remove(); });
    }

    // Expose server-rendered TempData toasts through the same pipeline for consistency.
    document.querySelectorAll('#toastContainer .toast').forEach(function (element) {
        new bootstrap.Toast(element, { delay: 4200 }).show();
    });

    var confirmModalElement = document.getElementById('appConfirmModal');
    var confirmModal = confirmModalElement ? new bootstrap.Modal(confirmModalElement) : null;
    var pendingConfirmation = null;

    if (confirmModalElement) {
        confirmModalElement.addEventListener('hidden.bs.modal', function () {
            if (pendingConfirmation) {
                pendingConfirmation(false);
                pendingConfirmation = null;
            }
        });

        confirmModalElement.addEventListener('shown.bs.modal', function () {
            var accept = document.getElementById('appConfirmAccept');
            if (accept) { accept.focus(); }
        });
    }

    function confirmDialog(title, message, options) {
        return new Promise(function (resolve) {
            if (!confirmModal) { resolve(window.confirm(message)); return; }
            pendingConfirmation = resolve;

            document.getElementById('appConfirmTitle').textContent = title || '¿Estás seguro?';
            document.getElementById('appConfirmMessage').textContent = message || '';
            var acceptButton = document.getElementById('appConfirmAccept');
            acceptButton.textContent = (options && options.confirmText) || 'Sí, continuar';
            acceptButton.className = 'btn flex-fill rounded-pill ' + ((options && options.tone) === 'primary' ? 'btn-primary' : 'btn-danger');

            var handler = function () {
                acceptButton.removeEventListener('click', handler);
                pendingConfirmation = null;
                confirmModal.hide();
                resolve(true);
            };
            acceptButton.removeEventListener('click', handler);
            acceptButton.addEventListener('click', handler);
            confirmModal.show();
        });
    }

    window.App = {
        toast: showToast,
        confirm: confirmDialog,
        getAntiForgeryToken: function () {
            var token = document.querySelector('input[name="__RequestVerificationToken"]');
            return token ? token.value : '';
        }
    };
})();

(function () {
    'use strict';

    // Reusable requisition-details modal loader (dashboard, pending list and history).
    var modalElement = document.getElementById('detailsModal');
    if (!modalElement) { return; }

    var modal = new bootstrap.Modal(modalElement);
    var content = document.getElementById('detailsModalContent');

    function loadDetails(id) {
        fetch('/Requisitions/Details/' + id, { headers: { 'X-Requested-With': 'fetch' } })
            .then(function (response) {
                if (!response.ok) { throw new Error('No se pudieron cargar los detalles.'); }
                return response.text();
            })
            .then(function (html) {
                content.innerHTML = html;
                modal.show();
            })
            .catch(function (error) {
                window.App.toast(error.message, 'error');
            });
    }

    document.addEventListener('click', function (event) {
        var trigger = event.target.closest('[data-load-details]');
        if (trigger) {
            loadDetails(trigger.getAttribute('data-load-details'));
            return;
        }

        var printTrigger = event.target.closest('[data-print-qr]');
        if (printTrigger) {
            printQr(printTrigger.getAttribute('data-print-qr'));
            return;
        }
    });

    function printQr(requisitionNumber) {
        var image = content.querySelector('.qr-image');
        if (!image) { return; }
        var printWindow = window.open('', '_blank', 'width=480,height=640');
        if (!printWindow) { return; }
        printWindow.document.write(
            '<!DOCTYPE html><html lang="es"><head><title>' + requisitionNumber + '</title>' +
            '<style>body{font-family:sans-serif;text-align:center;padding:40px}img{max-width:90%}h1{font-size:20px}</style>' +
            '</head><body><h1>' + requisitionNumber + '</h1>' +
            '<p>Escanea para surtir esta requisici\u00f3n</p>' +
            '<img src="' + image.src + '" onload="window.print();window.close();" />' +
            '</body></html>');
        printWindow.document.close();
    }
})();
