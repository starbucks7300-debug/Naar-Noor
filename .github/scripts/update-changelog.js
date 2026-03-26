#!/usr/bin/env node

const fs = require('fs');
const path = require('path');

/**
 * Auto-update CHANGELOG.md with recent changes
 */

function categorizeCommit(message) {
  const msg = message.toLowerCase();
  
  if (msg.startsWith('feat:') || msg.includes('add') || msg.includes('new')) {
    return 'Added';
  } else if (msg.startsWith('fix:') || msg.includes('fix') || msg.includes('bug')) {
    return 'Fixed';
  } else if (msg.startsWith('docs:') || msg.includes('documentation')) {
    return 'Documentation';
  } else if (msg.startsWith('style:') || msg.includes('style') || msg.includes('css')) {
    return 'Changed';
  } else if (msg.startsWith('refactor:') || msg.includes('refactor')) {
    return 'Changed';
  } else if (msg.startsWith('perf:') || msg.includes('performance')) {
    return 'Performance';
  } else if (msg.startsWith('test:') || msg.includes('test')) {
    return 'Testing';
  } else if (msg.includes('remove') || msg.includes('delete')) {
    return 'Removed';
  } else if (msg.includes('security')) {
    return 'Security';
  }
  
  return 'Changed';
}

function updateChangelog() {
  const changelogPath = path.join(process.cwd(), 'docs', 'CHANGELOG.md');
  
  if (!fs.existsSync(changelogPath)) {
    console.log('CHANGELOG.md not found, skipping...');
    return;
  }
  
  let content = fs.readFileSync(changelogPath, 'utf8');
  
  // Get commits from environment
  const commits = process.env.COMMITS ? process.env.COMMITS.split('\n').filter(Boolean) : [];
  const changedFiles = process.env.CHANGED_FILES ? process.env.CHANGED_FILES.split('\n').filter(Boolean) : [];
  
  if (commits.length === 0) {
    console.log('No commits to process');
    return;
  }
  
  // Categorize commits
  const categories = {
    'Added': [],
    'Fixed': [],
    'Changed': [],
    'Removed': [],
    'Security': [],
    'Performance': [],
    'Documentation': [],
    'Testing': []
  };
  
  commits.forEach(commit => {
    const category = categorizeCommit(commit);
    if (categories[category]) {
      categories[category].push(commit);
    }
  });
  
  // Generate unreleased section
  const timestamp = new Date().toISOString().split('T')[0];
  let unreleasedSection = `## [Unreleased]\n\nLast updated: ${timestamp}\n\n`;
  
  Object.keys(categories).forEach(category => {
    if (categories[category].length > 0) {
      unreleasedSection += `### ${category}\n\n`;
      categories[category].forEach(commit => {
        unreleasedSection += `${commit}\n`;
      });
      unreleasedSection += '\n';
    }
  });
  
  // Add changed files summary
  if (changedFiles.length > 0) {
    unreleasedSection += `### Modified Files\n\n`;
    changedFiles.slice(0, 10).forEach(file => {
      unreleasedSection += `- ${file}\n`;
    });
    if (changedFiles.length > 10) {
      unreleasedSection += `- ... and ${changedFiles.length - 10} more files\n`;
    }
    unreleasedSection += '\n';
  }
  
  // Update or insert unreleased section
  if (content.includes('## [Unreleased]')) {
    // Replace existing unreleased section
    content = content.replace(
      /## \[Unreleased\][\s\S]*?(?=## \[|$)/,
      unreleasedSection
    );
  } else {
    // Insert after the header
    const headerEnd = content.indexOf('\n## [');
    if (headerEnd !== -1) {
      content = content.slice(0, headerEnd) + '\n\n' + unreleasedSection + content.slice(headerEnd);
    } else {
      content += '\n\n' + unreleasedSection;
    }
  }
  
  fs.writeFileSync(changelogPath, content);
  console.log('✅ CHANGELOG.md updated successfully');
  console.log(`   - Processed ${commits.length} commits`);
  console.log(`   - Modified ${changedFiles.length} files`);
}

try {
  updateChangelog();
} catch (error) {
  console.error('❌ Error updating CHANGELOG.md:', error.message);
  process.exit(1);
}
