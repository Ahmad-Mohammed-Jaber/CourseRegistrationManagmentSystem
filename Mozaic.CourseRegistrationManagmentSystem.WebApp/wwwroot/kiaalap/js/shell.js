/* Kiaalap dashboard shell behaviour (ported from kiaalap/src/js/layout.js +
 * kiaalap/src/js/dashboard.js to plain script: no ES-module imports, uses the
 * global bootstrap bundle when present).
 */
(function () {
  'use strict';

  var MOBILE_BREAKPOINT = 768;
  var STORAGE_KEY = 'sidebarCollapsed';

  function getSidebar() { return document.getElementById('sidebar'); }
  function getMainWrapper() { return document.getElementById('mainWrapper'); }
  function getOverlay() { return document.getElementById('sidebarOverlay'); }

  function isMobile() { return window.innerWidth <= MOBILE_BREAKPOINT; }

  function setCollapsed(collapsed) {
    var sidebar = getSidebar();
    if (!sidebar) return;
    sidebar.classList.toggle('collapsed', collapsed);
    var mainWrapper = getMainWrapper();
    if (mainWrapper) mainWrapper.classList.toggle('full-width', collapsed);
    try { localStorage.setItem(STORAGE_KEY, String(collapsed)); } catch (e) { /* private mode */ }
  }

  function toggleSidebar() {
    var sidebar = getSidebar();
    if (!sidebar) return;
    setCollapsed(!sidebar.classList.contains('collapsed'));
  }

  function setMobileOpen(open) {
    var sidebar = getSidebar();
    var overlay = getOverlay();
    if (sidebar) sidebar.classList.toggle('active', open);
    if (overlay) overlay.classList.toggle('active', open);
  }

  function restoreSidebarState() {
    var sidebar = getSidebar();
    if (!sidebar || isMobile()) return;
    var stored = null;
    try { stored = localStorage.getItem(STORAGE_KEY); } catch (e) { return; }
    if (stored === 'true') {
      sidebar.classList.add('collapsed');
      var mainWrapper = getMainWrapper();
      if (mainWrapper) mainWrapper.classList.add('full-width');
    }
  }

  function initSidebar() {
    var sidebar = getSidebar();
    if (!sidebar) return;
    var sidebarToggle = document.getElementById('sidebarToggle');
    var sidebarClose = document.getElementById('sidebarClose');
    var overlay = getOverlay();
    var mainWrapper = getMainWrapper();

    restoreSidebarState();

    if (sidebarToggle) {
      sidebarToggle.addEventListener('click', function () {
        if (isMobile()) {
          setMobileOpen(true);
        } else {
          toggleSidebar();
        }
      });
    }
    if (sidebarClose) sidebarClose.addEventListener('click', function () { setMobileOpen(false); });
    if (overlay) overlay.addEventListener('click', function () { setMobileOpen(false); });

    document.addEventListener('keydown', function (e) {
      if (e.key === 'Escape' && sidebar.classList.contains('active')) setMobileOpen(false);
    });

    var resizeTimer;
    window.addEventListener('resize', function () {
      clearTimeout(resizeTimer);
      resizeTimer = setTimeout(function () {
        if (isMobile()) {
          sidebar.classList.remove('collapsed');
          if (mainWrapper) mainWrapper.classList.remove('full-width');
        } else {
          setMobileOpen(false);
          restoreSidebarState();
        }
      }, 250);
    });
  }

  function initSearch() {
    var searchToggle = document.getElementById('searchToggle');
    var searchForm = document.getElementById('searchForm');
    var searchInput = document.getElementById('searchInput');
    var closeSearch = document.getElementById('closeSearch');
    var searchBackdrop = document.getElementById('searchBackdrop');

    if (!searchToggle || !searchForm) return;

    function closeSearchBar() {
      searchForm.classList.remove('active');
      if (searchBackdrop) searchBackdrop.classList.remove('active');
      searchToggle.style.visibility = 'visible';
      if (searchInput) searchInput.value = '';
    }

    searchToggle.addEventListener('click', function (e) {
      e.stopPropagation();
      searchForm.classList.add('active');
      if (searchBackdrop) searchBackdrop.classList.add('active');
      searchToggle.style.visibility = 'hidden';
      setTimeout(function () { if (searchInput) searchInput.focus(); }, 300);
    });

    if (closeSearch) {
      closeSearch.addEventListener('click', function (e) {
        e.stopPropagation();
        closeSearchBar();
      });
    }
    if (searchBackdrop) searchBackdrop.addEventListener('click', closeSearchBar);
    searchForm.addEventListener('click', function (e) { e.stopPropagation(); });
    document.addEventListener('keydown', function (e) {
      if (e.key === 'Escape' && searchForm.classList.contains('active')) closeSearchBar();
    });
  }

  function initActiveMenuHighlighting() {
    var currentPage = window.location.pathname.split('/').pop() || 'index.html';
    var navLinks = document.querySelectorAll('.sidebar-nav .nav-link:not(.has-submenu)');
    navLinks.forEach(function (link) {
      var href = link.getAttribute('href');
      if (!href || href.charAt(0) === '#' || href.indexOf('javascript:') === 0) return;
      if (href !== currentPage) return;
      link.classList.add('active');
      var parentSubmenu = link.closest('.submenu');
      if (!parentSubmenu) return;
      parentSubmenu.classList.add('show');
      var parentToggle = document.querySelector('[data-bs-target="#' + parentSubmenu.id + '"]');
      if (parentToggle) {
        parentToggle.setAttribute('aria-expanded', 'true');
        parentToggle.classList.add('active');
      }
    });
  }

  function initBootstrapComponents() {
    if (!window.bootstrap) return;
    document.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(function (el) {
      new window.bootstrap.Tooltip(el);
    });
    document.querySelectorAll('[data-bs-toggle="popover"]').forEach(function (el) {
      new window.bootstrap.Popover(el);
    });
  }

  document.addEventListener('DOMContentLoaded', function () {
    initSidebar();
    initSearch();
    initActiveMenuHighlighting();
    initBootstrapComponents();
  });
})();
