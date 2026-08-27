(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', () => {
        const dataEl = document.getElementById('menu-data');
        if (!dataEl) return;

        /** @type {Array<{id:string,name:string,url:string,icon?:string,badge?:string,children:any[]}>} */
        const menuData = JSON.parse(dataEl.textContent || '[]');

        initDesktopCascade();
        initMobileDrawer(menuData);
    });

    // =========================================================
    // Desktop: nested cascading flyouts (nivel 1 → 2 → 3), each
    // level rendered server-side by _MenuLevel.cshtml. CSS alone
    // reveals submenus on hover; this just adds a short close
    // delay per branch so moving the mouse diagonally into a
    // flyout doesn't accidentally close it ("bermuda triangle").
    // =========================================================
    function initDesktopCascade() {
        const nav = document.querySelector('.category-nav');
        if (!nav) return;

        const closeTimers = new WeakMap();

        // Root trigger + panel behaves the same way as any branch.
        const root = nav.querySelector('.category-nav-inner');
        if (root) {
            root.addEventListener('mouseenter', () => {
                clearTimeout(closeTimers.get(root));
                root.classList.add('force-open');
            });
            root.addEventListener('mouseleave', () => {
                closeTimers.set(root, setTimeout(() => root.classList.remove('force-open'), 250));
            });
        }

        // Every branch that has a flyout, at any depth.
        nav.querySelectorAll('.cascade-item.has-children').forEach((li) => {
            li.addEventListener('mouseenter', () => {
                clearTimeout(closeTimers.get(li));
                // Only one open branch per level, so two flyouts don't stack.
                const siblings = li.parentElement ? li.parentElement.children : [];
                Array.from(siblings).forEach((sib) => {
                    if (sib !== li) sib.classList.remove('menu-open');
                });
                li.classList.add('menu-open');
            });
            li.addEventListener('mouseleave', () => {
                closeTimers.set(li, setTimeout(() => li.classList.remove('menu-open'), 250));
            });
        });

        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape') {
                nav.querySelectorAll('.menu-open').forEach((li) => li.classList.remove('menu-open'));
                root?.classList.remove('force-open');
            }
        });

        document.addEventListener('click', (e) => {
            if (root && !root.contains(e.target)) {
                nav.querySelectorAll('.menu-open').forEach((li) => li.classList.remove('menu-open'));
                root.classList.remove('force-open');
            }
        });
    }

    // =========================================================
    // Mobile: stacked (push/pop) drawer, same JSON tree, panels
    // are built lazily as the user drills into each level.
    // =========================================================
    function initMobileDrawer(menuData) {
        const hamburger = document.querySelector('.hamburger');
        const overlay = document.querySelector('.drawer-overlay');
        const drawer = document.querySelector('.side-drawer');
        const closeBtn = document.querySelector('.drawer-close');
        const panelsWrap = document.querySelector('.drawer-panels');
        if (!hamburger || !overlay || !drawer || !panelsWrap) return;

        /** @type {HTMLElement[]} */
        let panels = [];

        function buildPanel(items, showBack, title) {
            const panel = document.createElement('div');
            panel.className = 'drawer-panel';

            if (showBack) {
                const header = document.createElement('div');
                header.className = 'drawer-panel-header';

                const back = document.createElement('button');
                back.type = 'button';
                back.className = 'drawer-back';
                back.setAttribute('aria-label', 'Regresar');
                back.textContent = '←';
                back.addEventListener('click', popPanel);
                header.appendChild(back);

                const titleEl = document.createElement('span');
                titleEl.className = 'drawer-title';
                titleEl.textContent = title;
                header.appendChild(titleEl);

                panel.appendChild(header);
            }

            const ul = document.createElement('ul');
            ul.className = 'drawer-list';

            items.forEach(item => {
                const hasChildren = item.children && item.children.length > 0;
                const li = document.createElement('li');
                const row = document.createElement(hasChildren ? 'button' : 'a');
                row.type = hasChildren ? 'button' : undefined;
                row.className = 'drawer-item';
                if (!hasChildren) row.href = item.url || '#';

                if (item.icon) {
                    const icon = document.createElement('span');
                    icon.className = 'drawer-icon';
                    icon.textContent = item.icon;
                    row.appendChild(icon);
                }

                const label = document.createElement('span');
                label.textContent = item.name;
                row.appendChild(label);

                if (item.badge) {
                    const badge = document.createElement('span');
                    badge.className = 'drawer-badge';
                    badge.textContent = item.badge;
                    row.appendChild(badge);
                }

                if (hasChildren) {
                    const chevron = document.createElement('span');
                    chevron.className = 'drawer-chevron';
                    chevron.textContent = '›';
                    row.appendChild(chevron);
                    row.addEventListener('click', () => pushPanel(item.children, item.name));
                }

                li.appendChild(row);
                ul.appendChild(li);
            });

            panel.appendChild(ul);
            return panel;
        }

        function showRoot() {
            panelsWrap.innerHTML = '';
            const root = buildPanel(menuData, false, 'Categorías');
            root.style.transform = 'translateX(0)';
            panelsWrap.appendChild(root);
            panels = [root];
        }

        function pushPanel(items, title) {
            const incoming = buildPanel(items, true, title);
            incoming.style.transform = 'translateX(100%)';
            panelsWrap.appendChild(incoming);

            // force reflow so the transition below actually animates
            void incoming.offsetWidth;

            requestAnimationFrame(() => {
                incoming.style.transform = 'translateX(0)';
            });

            panels.push(incoming);
        }

        function popPanel() {
            if (panels.length <= 1) return;
            const top = panels.pop();
            top.style.transform = 'translateX(100%)';
            setTimeout(() => top.remove(), 300);
        }

        function openDrawer() {
            showRoot();
            drawer.classList.add('open');
            overlay.classList.add('open');
            drawer.setAttribute('aria-hidden', 'false');
            hamburger.setAttribute('aria-expanded', 'true');
            document.body.style.overflow = 'hidden';
        }

        function closeDrawer() {
            drawer.classList.remove('open');
            overlay.classList.remove('open');
            drawer.setAttribute('aria-hidden', 'true');
            hamburger.setAttribute('aria-expanded', 'false');
            document.body.style.overflow = '';
        }

        hamburger.addEventListener('click', openDrawer);
        closeBtn?.addEventListener('click', closeDrawer);
        overlay.addEventListener('click', closeDrawer);
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && drawer.classList.contains('open')) closeDrawer();
        });

        // If the viewport crosses into desktop while the drawer is open, close it.
        window.matchMedia('(min-width: 992px)').addEventListener('change', (e) => {
            if (e.matches) closeDrawer();
        });
    }

    function escapeHtml(str) {
        const div = document.createElement('div');
        div.textContent = str ?? '';
        return div.innerHTML;
    }
})();
