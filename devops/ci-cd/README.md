# CI/CD - GitHub Actions

**⚠️ IMPORTANT**: CI/CD workflows are stored in `.github/workflows/` (GitHub requirement).

This folder is a reference guide for the CI/CD pipeline structure.

## Actual Workflow Files Location

All GitHub Actions workflows are in: `/.github/workflows/`

GitHub Actions **must** be in `.github/workflows/` to be automatically discovered and executed.

## CI/CD Pipeline Overview

### Workflows (in `.github/workflows/`)

1. **00-secret-scan.yml** - Detect secrets before commit
   - Scans for API keys, passwords, tokens
   - Blocks commits with exposed secrets

2. **01-unit-tests.yml** - Run unit tests
   - Backend: .NET tests
   - Frontend: JavaScript/TypeScript tests
   - Mobile: React Native tests
   - Desktop: WinForms tests

3. **02-integration-tests.yml** - Integration testing
   - API + Database tests
   - Cross-service communication
   - End-to-end workflows

4. **03-coverage-analysis.yml** - Code coverage reports
   - Minimum coverage threshold (80%)
   - Generate coverage badges
   - Track coverage trends

5. **04-build-artifacts.yml** - Build Docker images
   - Backend image (ghcr.io/.../backend)
   - Frontend image (ghcr.io/.../frontend)
   - Push to GitHub Container Registry

6. **05-deploy.yml** - Deploy to infrastructure
   - Update Kubernetes deployments
   - Apply Terraform changes
   - Rolling updates with health checks

7. **06-sast-sca.yml** - Security scanning
   - SAST: Static Application Security Testing
   - SCA: Software Composition Analysis
   - Dependency vulnerability scanning

8. **07-lighthouse-ci.yml** - Performance & accessibility
   - Lighthouse CI for web performance
   - Accessibility audits
   - SEO checks

## Triggering Workflows

Workflows are triggered on:
- **Push** to main branch
- **Pull requests** to main
- **Schedule** (daily/weekly for certain jobs)
- **Manual** workflow dispatch (select workflows)

## Accessing Workflow Runs

View workflow runs in GitHub:
1. Go to repository
2. Click "Actions" tab
3. Select workflow to see runs
4. Click run to see details and logs

## For DevOps Team

To modify workflows:
1. Edit files in `.github/workflows/`
2. Commit and push changes
3. Workflows take effect on next push/PR

Example:
```bash
# Edit a workflow
nano .github/workflows/05-deploy.yml

# Commit changes
git add .github/workflows/05-deploy.yml
git commit -m "ci: Update deployment workflow"
git push
```

## Pipeline Flow

```
Code Push → GitHub
     ↓
Secret Scan (00)
     ↓
Unit Tests (01)
     ↓
Integration Tests (02)
     ↓
Coverage Analysis (03)
     ↓
Build Artifacts (04)
     ↓
Security Scan (06)
     ↓
Performance Test (07)
     ↓
Deploy (05) [if all pass]
```

## Environment Configuration

Secrets and variables are configured in GitHub:
- Repository Settings → Secrets and variables
- Organization Settings → Secrets (shared)

Required secrets:
- `GHCR_TOKEN` - Container registry authentication
- `KUBECONFIG` - Kubernetes cluster access
- `TERRAFORM_*` - Terraform variables
- `AWS_*` or `AZURE_*` - Cloud provider credentials

## Monitoring & Debugging

### Check workflow status
```bash
# Use GitHub CLI
gh run list --workflow=01-unit-tests.yml

# View specific run
gh run view <run-id>

# View logs
gh run view <run-id> --log
```

### Common Issues

**Workflow not running:**
- Check `.github/workflows/` exists
- Verify file extension is `.yml` or `.yaml`
- Check branch name matches trigger conditions

**Secret not found:**
- Verify secret name in GitHub Settings
- Check secret scope (repo vs org)
- Ensure GitHub token has correct permissions

**Build failures:**
- Check build logs in GitHub Actions
- Review recent code changes
- Verify Docker images build locally

## Deployment Trigger

The `05-deploy.yml` workflow automatically deploys when:
1. All tests pass
2. All scans pass
3. Docker images build successfully
4. Deployment update succeeds

Manual trigger:
```bash
gh workflow run 05-deploy.yml -f environment=production
```

---

**Location**: `.github/workflows/` (GitHub requirement)  
**Reference**: This folder (documentation only)  
**Status**: Production ready
