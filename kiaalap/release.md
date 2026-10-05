# v2.3.0 - Dependency Updates, ESLint v10 Migration & Fixes

All dependencies updated to their latest versions. ESLint migrated to v10 flat config. Sidebar and charts improvements.

## Highlights

- **All packages updated** to latest versions (20+ dependencies)
- **ESLint v10 migration** from legacy `.eslintrc.js` to flat config `eslint.config.js`
- **Sidebar improvements** with better collapse behavior and responsive handling
- **Charts initialization** updated for improved responsiveness
- **Deployment guide** added with Apache/Nginx configuration examples
- **Fixed** broken README screenshot (replaced external Colorlib URL with local image)

## Dependency Updates

### Runtime Dependencies

| Package | Previous | Updated |
|---------|----------|---------|
| @fortawesome/fontawesome-free | 7.0.1 | **7.2.0** |
| chart.js | 4.5.0 | **4.5.1** |
| countup.js | 2.8.0 | **2.9.0** |
| dayjs | 1.11.10 | **1.11.19** |
| filepond | 4.30.6 | **4.32.11** |
| fullcalendar | 6.1.11 | **6.1.20** |
| metismenu | 3.0.6 | **3.1.0** |
| sass | 1.92.1 | **1.97.3** |
| simple-datatables | 10.0.0 | **10.2.0** |
| simplebar | 6.2.5 | **6.3.3** |
| swiper | 12.0.2 | **12.1.0** |
| tom-select | 2.3.1 | **2.4.6** |

### Dev Dependencies

| Package | Previous | Updated |
|---------|----------|---------|
| autoprefixer | 10.4.21 | **10.4.24** |
| cssnano | 7.1.1 | **7.1.2** |
| eslint | 9.35.0 | **10.0.0** (major) |
| glob | 13.0.0 | **13.0.2** |
| html-validate | 10.0.0 | **10.7.0** |
| prettier | 3.6.2 | **3.8.1** |
| stylelint | 16.24.0 | **17.2.0** (major) |
| stylelint-config-standard-scss | 16.0.0 | **17.0.0** (major) |
| terser | 5.44.0 | **5.46.0** |
| vite | 7.1.7 | **7.3.1** |

### New Dev Dependencies

| Package | Version | Notes |
|---------|---------|-------|
| @eslint/js | 10.0.1 | Required for ESLint v10 flat config |
| globals | 17.3.0 | Browser globals for ESLint v10 flat config |

## What's Changed

- Enhanced sidebar collapse behavior with localStorage persistence and improved responsive handling
- Updated charts initialization and sidebar navigation for better responsiveness
- Added comprehensive deployment guide (`DEPLOYMENT_GUIDE.md`) with Apache and Nginx configs
- Added Image Cropper link to sidebar navigation
- Fixed broken README dashboard preview image (now served locally)
- Migrated ESLint from legacy config to v10 flat config format

## Technical Notes

- **CropperJS** kept at v1.6.2 (v2.x has a completely different web components API)
- **Quill** kept at v2.0.3 (latest stable)
- All builds verified to complete successfully with updated dependencies

## Upgrading from v2.2.0

```bash
npm install
```

If you have a custom ESLint config using the legacy `.eslintrc.js` format, you'll need to migrate it to `eslint.config.js` (flat config). No other breaking changes.

## Full Changelog

See [CHANGELOG.md](CHANGELOG.md) for complete version history.
