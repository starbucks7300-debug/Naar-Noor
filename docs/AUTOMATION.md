# Documentation Automation

Complete guide to the automated documentation system for The Lost Yeti Kitchen & Bar project.

## Overview

This project uses GitHub Actions and Node.js scripts to automatically update documentation whenever code changes are made. This ensures documentation stays synchronized with the codebase without manual intervention.

## What Gets Automated

### 1. Project Structure (STRUCTURE.md)
**Updates When**:
- Files added/removed in `src/`
- Configuration files change (`package.json`, `angular.json`, `tsconfig.json`)

**What Updates**:
- File tree structure
- Component count
- TypeScript file count
- HTML template count
- CSS stylesheet count
- Last updated timestamp

### 2. Features Documentation (FEATURES.md)
**Updates When**:
- New components created
- Component inputs/outputs change
- Component files modified

**What Updates**:
- Component inventory
- Component selectors
- Input/Output properties
- Component locations
- Total component count

### 3. Changelog (CHANGELOG.md)
**Updates When**:
- Code pushed to main branch
- Pull requests merged

**What Updates**:
- Unreleased section with categorized commits
- Modified files list
- Commit history
- Timestamp

### 4. Technology Stack (TECHNOLOGIES.md)
**Updates When**:
- Dependencies added/updated/removed
- `package.json` changes

**What Updates**:
- Dependency versions
- Angular version
- Dev dependencies
- Build configuration
- Browser support list

### 5. README (README.md)
**Updates When**:
- Any code changes
- Components added

**What Updates**:
- Project statistics badges
- Component count
- File counts
- Last updated timestamp

### 6. Sitemap (sitemap.xml)
**Updates When**:
- Routes change in `app.routes.ts`
- New sections added

**What Updates**:
- URL list
- Last modified dates
- Priority levels
- Change frequency

### 7. API Documentation
**Updates When**:
- Code pushed to main branch

**What Updates**:
- Component API docs
- Service documentation
- Interface definitions
- Generated with Compodoc

## GitHub Actions Workflows

### docs-update.yml
Main documentation workflow with multiple jobs:

```yaml
Triggers:
  - push to main/develop
  - pull requests
  - changes to src/**

Jobs:
  1. update-structure-docs
  2. update-changelog
  3. update-dependencies-docs
  4. update-readme
  5. generate-api-docs
```

**Features**:
- Parallel job execution
- Conditional execution based on changes
- Automatic commit with `[skip ci]`
- Error handling and logging

### sitemap-update.yml
Dedicated sitemap workflow:

```yaml
Triggers:
  - changes to app.routes.ts
  - changes to component files
  - manual dispatch

Jobs:
  1. update-sitemap
  2. validate-sitemap
  3. commit-changes
```

**Features**:
- XML validation
- Route extraction
- Section discovery
- Automatic commit

## How It Works

### Workflow Execution

```
1. Developer pushes code
        ↓
2. GitHub detects changes
        ↓
3. Workflow triggers
        ↓
4. Scripts analyze codebase
        ↓
5. Documentation updated
        ↓
6. Changes committed
        ↓
7. Repository updated
```

### Commit Flow

```
Code Change → Workflow → Script → Update → Commit [skip ci]
```

The `[skip ci]` flag prevents infinite loops by skipping CI on documentation commits.

## Scripts

All automation scripts are in `.github/scripts/`:

| Script | Purpose | Input | Output |
|--------|---------|-------|--------|
| `update-structure.js` | Project structure | File system | STRUCTURE.md |
| `update-features.js` | Component inventory | Component files | FEATURES.md |
| `update-changelog.js` | Commit history | Git log | CHANGELOG.md |
| `update-technologies.js` | Dependencies | package.json | TECHNOLOGIES.md |
| `update-readme.js` | Statistics | File counts | README.md |
| `update-sitemap.js` | Site URLs | Routes/sections | sitemap.xml |

## Manual Execution

### Run Individual Scripts

```bash
# Update structure documentation
npm run docs:structure

# Update features documentation
npm run docs:features

# Update changelog
npm run docs:changelog

# Update technology stack
npm run docs:technologies

# Update README
npm run docs:readme

# Update sitemap
npm run docs:sitemap
```

### Run All Scripts

```bash
npm run docs:update
```

### Direct Execution

```bash
node .github/scripts/update-structure.js
node .github/scripts/update-features.js
node .github/scripts/update-changelog.js
node .github/scripts/update-technologies.js
node .github/scripts/update-readme.js
node .github/scripts/update-sitemap.js
```

## Configuration

### Workflow Triggers

Edit `.github/workflows/docs-update.yml`:

```yaml
on:
  push:
    branches:
      - main
      - develop
    paths:
      - 'src/**'
      - 'package.json'
```

### Script Behavior

Each script can be customized by editing the corresponding file in `.github/scripts/`.

### Commit Messages

Automated commits use these formats:
- `docs: auto-update structure and features documentation [skip ci]`
- `docs: auto-update changelog [skip ci]`
- `docs: auto-update dependencies documentation [skip ci]`
- `docs: auto-update README with latest stats [skip ci]`
- `chore: auto-update sitemap.xml [skip ci]`

## Best Practices

### 1. Review Automated Commits
Always review what the automation changed:
```bash
git log --oneline --author="github-actions[bot]"
```

### 2. Test Locally First
Before pushing, test scripts locally:
```bash
npm run docs:update
git diff docs/
```

### 3. Keep Scripts Updated
When project structure changes, update scripts accordingly.

### 4. Monitor Workflow Runs
Check GitHub Actions tab for:
- Failed runs
- Warnings
- Execution time

### 5. Handle Merge Conflicts
If documentation conflicts occur:
```bash
git pull --rebase
npm run docs:update
git add docs/
git rebase --continue
```

## Troubleshooting

### Workflow Not Triggering

**Check**:
- Trigger paths in workflow file
- Branch protection rules
- Workflow permissions

**Solution**:
```yaml
permissions:
  contents: write
```

### Script Fails

**Check**:
- Node.js version (requires 18+)
- File paths exist
- Syntax errors

**Debug**:
```bash
node --version
npm run docs:structure
```

### No Changes Committed

**Check**:
- Files actually changed
- Git configuration
- Commit permissions

**Solution**:
```yaml
- uses: stefanzweifel/git-auto-commit-action@v5
  with:
    commit_message: "docs: update [skip ci]"
```

### Infinite Loop

**Cause**: Workflow triggers on its own commits

**Solution**: Always use `[skip ci]` in commit messages

### Merge Conflicts

**Cause**: Multiple updates to same documentation

**Solution**:
1. Pull latest changes
2. Resolve conflicts
3. Re-run scripts
4. Commit resolved version

## Advanced Usage

### Custom Triggers

Add manual workflow dispatch:

```yaml
on:
  workflow_dispatch:
    inputs:
      docs_type:
        description: 'Documentation to update'
        required: true
        type: choice
        options:
          - all
          - structure
          - features
          - changelog
```

### Scheduled Updates

Run documentation updates daily:

```yaml
on:
  schedule:
    - cron: '0 0 * * *'  # Daily at midnight
```

### Conditional Execution

Only run on specific changes:

```yaml
jobs:
  update-docs:
    if: contains(github.event.head_commit.message, 'feat:')
```

### Notifications

Add Slack/Discord notifications:

```yaml
- name: Notify on failure
  if: failure()
  uses: 8398a7/action-slack@v3
  with:
    status: ${{ job.status }}
```

## Security Considerations

### Token Permissions

Workflows use `GITHUB_TOKEN` with minimal permissions:
- Read repository content
- Write to repository (for commits)

### Script Safety

All scripts:
- Run in isolated environment
- Don't execute user input
- Validate file paths
- Handle errors gracefully

### Commit Verification

Automated commits are signed by `github-actions[bot]`.

## Performance

### Optimization Tips

1. **Parallel Jobs**: Run independent updates simultaneously
2. **Conditional Execution**: Skip unnecessary runs
3. **Caching**: Cache npm dependencies
4. **Minimal Checkouts**: Use shallow clones

### Execution Time

Typical workflow duration:
- Structure update: ~30 seconds
- Features update: ~45 seconds
- Changelog update: ~20 seconds
- Full documentation: ~2-3 minutes

## Future Enhancements

### Planned Features

1. **Visual Diff**: Show documentation changes in PR comments
2. **Coverage Reports**: Track documentation coverage
3. **Link Validation**: Check for broken links
4. **Spell Checking**: Automated spell check
5. **Version Tagging**: Auto-tag documentation versions
6. **PDF Generation**: Generate PDF documentation
7. **Multi-language**: Support for i18n documentation

### Integration Ideas

- Integrate with project management tools
- Auto-create issues for missing docs
- Generate release notes automatically
- Sync with external documentation platforms

## Contributing

When contributing to automation:

1. **Test Thoroughly**: Run scripts locally
2. **Document Changes**: Update this file
3. **Follow Conventions**: Match existing patterns
4. **Add Error Handling**: Catch and log errors
5. **Update Workflows**: Modify GitHub Actions if needed

## Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Node.js File System API](https://nodejs.org/api/fs.html)
- [Compodoc Documentation](https://compodoc.app/)
- [Semantic Versioning](https://semver.org/)
- [Keep a Changelog](https://keepachangelog.com/)

## Support

For issues with automation:
1. Check workflow logs in GitHub Actions
2. Run scripts locally for debugging
3. Review script source code
4. Open an issue with error details

---

**Last Updated**: 2026-03-26

**Maintained By**: Development Team

**Status**: Active and Maintained
