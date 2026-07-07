// Sticky horizontal scrollbar functionality
// Ez a script biztosítja, hogy a táblázat vízszintes scrollbarja mindig látható maradjon

export function initializeStickyScroll(tableWrapperSelector) {
    const wrappers = document.querySelectorAll(tableWrapperSelector);

    wrappers.forEach(wrapper => {
        if (!wrapper) return;

        // Létrehozunk egy shadow scrollbart, ami az oldal alján marad
        const shadowScroll = document.createElement('div');
        shadowScroll.className = 'sticky-scrollbar-shadow';
        shadowScroll.style.position = 'fixed';
        shadowScroll.style.bottom = '0';
        shadowScroll.style.left = '0';
        shadowScroll.style.right = '0';
        shadowScroll.style.height = '17px';
        shadowScroll.style.overflowX = 'auto';
        shadowScroll.style.overflowY = 'hidden';
        shadowScroll.style.zIndex = '1000';
        shadowScroll.style.background = '#f5f5f5';
        shadowScroll.style.borderTop = '1px solid #ddd';
        shadowScroll.style.display = 'none';

        // Shadow scrollbar tartalma (ugyanakkora szélesség, mint az eredeti tartalom)
        const shadowContent = document.createElement('div');
        shadowScroll.appendChild(shadowContent);
        document.body.appendChild(shadowScroll);

        // Szinkronizálás funkciók
        const syncShadowToWrapper = () => {
            wrapper.scrollLeft = shadowScroll.scrollLeft;
        };

        const syncWrapperToShadow = () => {
            shadowScroll.scrollLeft = wrapper.scrollLeft;
        };

        const updateShadowSize = () => {
            const scrollWidth = wrapper.scrollWidth;
            shadowContent.style.width = scrollWidth + 'px';
            shadowContent.style.height = '1px';
        };

        const checkVisibility = () => {
            const rect = wrapper.getBoundingClientRect();
            const wrapperBottom = rect.bottom;
            const windowHeight = window.innerHeight;

            // Csak akkor jelenítjük meg a shadow scrollbart, ha a wrapper scrollable
            const needsScroll = wrapper.scrollWidth > wrapper.clientWidth;

            // Ha a wrapper alja nincs látható tartományban és szükséges a scroll
            if (wrapperBottom > windowHeight && needsScroll) {
                shadowScroll.style.display = 'block';
                updateShadowSize();
            } else {
                shadowScroll.style.display = 'none';
            }
        };

        // Event listenerek
        shadowScroll.addEventListener('scroll', syncShadowToWrapper);
        wrapper.addEventListener('scroll', syncWrapperToShadow);

        // Frissítés változáskor
        const observer = new ResizeObserver(() => {
            updateShadowSize();
            checkVisibility();
        });
        observer.observe(wrapper);

        // Frissítés görgetéskor és resize-oláskor
        window.addEventListener('scroll', checkVisibility);
        window.addEventListener('resize', () => {
            updateShadowSize();
            checkVisibility();
        });

        // Kezdeti ellenőrzés
        setTimeout(() => {
            updateShadowSize();
            checkVisibility();
        }, 100);

        // Cleanup funkció tárolása a wrapper-en
        wrapper._stickyScrollCleanup = () => {
            shadowScroll.remove();
            observer.disconnect();
            window.removeEventListener('scroll', checkVisibility);
        };
    });
}

export function cleanupStickyScroll(tableWrapperSelector) {
    const wrappers = document.querySelectorAll(tableWrapperSelector);
    wrappers.forEach(wrapper => {
        if (wrapper._stickyScrollCleanup) {
            wrapper._stickyScrollCleanup();
        }
    });
}

// Auto-initialize ha a modul importálva van
if (typeof window !== 'undefined') {
    window.initializeStickyScroll = initializeStickyScroll;
    window.cleanupStickyScroll = cleanupStickyScroll;
}
