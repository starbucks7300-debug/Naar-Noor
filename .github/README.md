# GitHub Configuration

This directory contains all GitHub-specific configuration files for The Lost Yeti Kitchen & Bar project.

## 📁 Directory Structure

```
.github/
├── ISSUE_TEMPLATE/          # Issue templates
│   ├── bug_report.yml       # Bug report template
│   ├── feature_request.yml  # Feature request template
│   └── config.yml           # Issue template configuration
├── scripts/                 # Automation scripts
│   ├── update-structure.js
│   ├── update-features.js
│   ├── update-changelog.js
│   ├── update-technologies.js
│   ├── update-readme.js
│   ├── update-sitemap.js
│   └── README.md
├── workflows/               # GitHub Actions workflows
│   ├── ci.yml              # Continuous Integration
│   ├── deploy.yml          # Deployment pipeline
│   ├── security.yml        # Security scanning
│   ├── docs-update.yml     # Documentation automation
│   ├── sitemap-update.yml  # Sitemap automation
│   ├── labeler.yml         # Auto-labeling PRs
│   ├── stale.yml           # Stale issue management
│   └── release.yml         # Release automation
├── CODEOWNERS              # Code ownership rules
├── dependabot.yml          # Dependency updates
├── FUNDING.yml             # Funding information
├── labeler.yml             # PR labeling rules
├── pull_request_template.md # PR template
└── README.md               # This file
```

## 🔄 Workflows

### CI Pipeline (ci.yml)
**Triggers**: Push to main/develop, Pull requests

**Jobs**:
- **Lint**: Code quality checks with ESLint
- **Test**: Unit tests with coverage
- **Build**: Production build
- **Security**: npm audit and Snyk scan
- **Lighthouse**: Performance testing
- **Notify**: Status notifications

**Status**: ✅ Active

### Deployment (deploy.yml)
**Triggers**: Push to main, Tags (v*), Manual dispatch

**Jobs**:
- **Build**: Production build
- **Docker**: Build and push Docker image
- **Vercel**: Deploy to Vercel
- **Netlify**: Deploy to Netlify
- **AWS S3**: Deploy to S3 + CloudFront
- **GitHub Pages**: Deploy to GitHub Pages
- **Smoke Test**: Post-deployment validation
- **Notify**: Deployment status

**Status**: ✅ Active

### Security Scan (security.yml)
**Triggers**: Weekly schedule, Push to main, Pull requests, Manual

**Jobs**:
- **Dependency Scan**: npm audit
- **Snyk Scan**: Vulnerability scanning
- **CodeQL**: Code analysis
- **Trivy**: Container scanning
- **OWASP**: Dependency check
- **Secret Scan**: TruffleHog
- **License Check**: License compliance
- **Report**: Generate security summary

**Status**: ✅ Active

### Documentation Update (docs-update.yml)
**Triggers**: Push to main/develop, Changes to src/**, package.json

**Jobs**:
- **Structure**: Update STRUCTURE.md
- **Features**: Update FEATURES.md
- **Changelog**: Update CHANGELOG.md
- **Technologies**: Update TECHNOLOGIES.md
- **README**: Update README.md
- **API Docs**: Generate with Compodoc

**Status**: ✅ Active

### Sitemap Update (sitemap-update.yml)
**Triggers**: Changes to routes, Manual dispatch

**Jobs**:
- **Update**: Regenerate sitemap.xml
- **Validate**: XML validation
- **Commit**: Auto-commit changes

**Status**: ✅ Active

### Auto Labeler (labeler.yml)
**Triggers**: Pull request opened/updated

**Jobs**:
- **Label**: Automatically label PRs based on changed files

**Status**: ✅ Active

### Stale Management (stale.yml)
**Triggers**: Daily schedule, Manual dispatch

**Jobs**:
- **Stale**: Mark and close stale issues/PRs

**Configuration**:
- Issues: 60 days stale, 7 days to close
- PRs: 30 days stale, 14 days to close

**Status**: ✅ Active

### Release (release.yml)
**Triggers**: Tags (v*.*.*), Manual dispatch

**Jobs**:
- **Build**: Create release assets
- **Changelog**: Generate changelog
- **Release**: Create GitHub release
- **Docker**: Push Docker image
- **npm**: Publish to npm (optional)
- **Notify**: Release notifications

**Status**: ✅ Active

## 📝 Issue Templates

### Bug Report (bug_report.yml)
Structured form for reporting bugs with:
- Description
- Steps to reproduce
- Expected vs actual behavior
- Screenshots
- Browser/device information
- Additional context

### Feature Request (feature_request.yml)
Structured form for suggesting features with:
- Problem statement
- Proposed solution
- Alternatives considered
- Priority level
- Category
- Mockups/examples

### Configuration (config.yml)
- Disables blank issues
- Links to discussions
- Links to documentation
- Links to security advisories

## 🏷️ Labels

Automatic labels applied by `labeler.yml`:

| Label | Applied When |
|-------|-------------|
| `documentation` | Changes to docs/ or *.md files |
| `source` | Changes to src/ files |
| `components` | Changes to component files |
| `styles` | Changes to CSS/SCSS files |
| `typescript` | Changes to *.ts files |
| `templates` | Changes to *.html files |
| `configuration` | Changes to config files |
| `github-actions` | Changes to workflows |
| `scripts` | Changes to automation scripts |
| `docker` | Changes to Docker files |
| `dependencies` | Changes to package files |
| `assets` | Changes to assets |
| `tests` | Changes to test files |
| `seo` | Changes to SEO files |
| `security` | Changes to security files |

## 👥 Code Owners

Defined in `CODEOWNERS`:
- Default: @yourusername
- Documentation: @yourusername
- GitHub config: @yourusername
- Source code: @yourusername
- Components: @yourusername
- Configuration: @yourusername
- Docker: @yourusername
- Security: @yourusername

## 🤖 Dependabot

Configured in `dependabot.yml`:

### npm Dependencies
- **Schedule**: Weekly (Monday 9:00 AM)
- **Limit**: 10 open PRs
- **Groups**: Angular packages, development dependencies
- **Ignores**: Angular major versions (manual review)

### GitHub Actions
- **Schedule**: Weekly (Monday 9:00 AM)
- **Limit**: 5 open PRs

### Docker
- **Schedule**: Weekly (Monday 9:00 AM)
- **Limit**: 5 open PRs

## 🔧 Scripts

All automation scripts are in `.github/scripts/`:

| Script | Purpose |
|--------|---------|
| `update-structure.js` | Update project structure docs |
| `update-features.js` | Update component inventory |
| `update-changelog.js` | Update changelog with commits |
| `update-technologies.js` | Update tech stack docs |
| `update-readme.js` | Update README badges |
| `update-sitemap.js` | Regenerate sitemap |

See [scripts/README.md](scripts/README.md) for details.

## 📋 Pull Request Template

Located in `pull_request_template.md`:

**Sections**:
- Description
- Type of change
- Related issues
- Changes made
- Screenshots (before/after)
- Testing checklist
- Code quality checklist
- Performance impact
- Breaking changes
- Deployment notes
- Reviewer checklist

## 💰 Funding

Configured in `FUNDING.yml`:
- GitHub Sponsors
- Open Collective
- Ko-fi
- Patreon
- Buy Me a Coffee

## 🚀 Usage

### Running Workflows Manually

```bash
# Trigger documentation update
gh workflow run docs-update.yml

# Trigger security scan
gh workflow run security.yml

# Trigger deployment
gh workflow run deploy.yml

# Create a release
gh workflow run release.yml -f version=v1.0.0
```

### Creating Issues

Use the issue templates:
1. Go to Issues → New Issue
2. Select template (Bug Report or Feature Request)
3. Fill out the form
4. Submit

### Creating Pull Requests

1. Create a branch
2. Make changes
3. Push to GitHub
4. Create PR (template auto-fills)
5. Fill out all sections
6. Request review

### Managing Stale Items

Stale bot runs automatically, but you can:
- Add `pinned` label to prevent staleness
- Add `work-in-progress` label for active PRs
- Comment on stale items to reactivate them

## 🔒 Security

### Secrets Required

For full functionality, configure these secrets:

| Secret | Purpose | Required For |
|--------|---------|-------------|
| `GITHUB_TOKEN` | GitHub API | All workflows (auto-provided) |
| `SNYK_TOKEN` | Snyk scanning | Security workflow |
| `DOCKER_USERNAME` | Docker Hub | Deployment |
| `DOCKER_PASSWORD` | Docker Hub | Deployment |
| `VERCEL_TOKEN` | Vercel deployment | Deployment |
| `VERCEL_ORG_ID` | Vercel org | Deployment |
| `VERCEL_PROJECT_ID` | Vercel project | Deployment |
| `NETLIFY_AUTH_TOKEN` | Netlify deployment | Deployment |
| `NETLIFY_SITE_ID` | Netlify site | Deployment |
| `AWS_ACCESS_KEY_ID` | AWS deployment | Deployment |
| `AWS_SECRET_ACCESS_KEY` | AWS deployment | Deployment |
| `AWS_S3_BUCKET` | S3 bucket name | Deployment |
| `AWS_CLOUDFRONT_DISTRIBUTION_ID` | CloudFront ID | Deployment |
| `SLACK_WEBHOOK` | Slack notifications | Notifications |
| `NPM_TOKEN` | npm publishing | Release (optional) |

### Setting Secrets

```bash
# Using GitHub CLI
gh secret set SECRET_NAME

# Or via GitHub UI
Settings → Secrets and variables → Actions → New repository secret
```

## 📊 Monitoring

### Workflow Status

Check workflow status:
```bash
# List workflow runs
gh run list

# View specific run
gh run view <run-id>

# Watch a running workflow
gh run watch
```

### Badges

Add workflow badges to README:

```markdown
![CI](https://github.com/yourusername/lost-yeti-angular/workflows/CI%20Pipeline/badge.svg)
![Deploy](https://github.com/yourusername/lost-yeti-angular/workflows/Deploy%20to%20Production/badge.svg)
![Security](https://github.com/yourusername/lost-yeti-angular/workflows/Security%20Scan/badge.svg)
```

## 🛠️ Maintenance

### Updating Workflows

1. Edit workflow file in `.github/workflows/`
2. Test locally if possible
3. Commit and push
4. Monitor first run
5. Adjust as needed

### Updating Scripts

1. Edit script in `.github/scripts/`
2. Test locally: `node .github/scripts/script-name.js`
3. Update documentation if needed
4. Commit and push

### Updating Templates

1. Edit template file
2. Test by creating issue/PR
3. Adjust based on feedback
4. Document changes

## 📚 Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Dependabot Documentation](https://docs.github.com/en/code-security/dependabot)
- [Issue Templates](https://docs.github.com/en/communities/using-templates-to-encourage-useful-issues-and-pull-requests)
- [CODEOWNERS](https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features/customizing-your-repository/about-code-owners)

## 🤝 Contributing

When contributing to GitHub configuration:

1. Test changes thoroughly
2. Document new workflows/scripts
3. Update this README
4. Follow existing patterns
5. Consider security implications

## 📞 Support

For issues with GitHub configuration:
1. Check workflow logs
2. Review documentation
3. Test locally when possible
4. Open an issue with details

---

**Last Updated**: 2026-03-26

**Maintained By**: Development Team

**Status**: Active and Maintained
