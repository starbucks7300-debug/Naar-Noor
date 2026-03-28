#!/usr/bin/env node

const fs = require('fs');
const path = require('path');

/**
 * Auto-update README.md with project statistics
 */

function countFiles(dir, extension = null) {
  let count = 0;
  if (!fs.existsSync(dir)) return count;
  const items = fs.readdirSync(dir, { withFileTypes: true });
  
  items.forEach(item => {
    const fullPath = path.join(dir, item.name);
    if (item.isDirectory() && !['node_modules', '.angular', 'dist', '.git'].includes(item.name)) {
      count += countFiles(fullPath, extension);
    } else if (item.isFile()) {
      if (!extension || item.name.endsWith(extension)) {
        count++;
      }
    }
  });
  
  return count;
}

function updateReadme() {
  const readmePath = path.join(process.cwd(), 'README.md');
  const srcPath = path.join(process.cwd(), 'src');
  const appPath = path.join(srcPath, 'app');
  
  if (!fs.existsSync(readmePath)) {
    console.log('README.md not found, skipping...');
    return;
  }
  
  let content = fs.readFileSync(readmePath, 'utf8');
  
  // Get stats by counting files
  const stats = {
    totalFiles: countFiles(process.cwd()).toString(),
    tsFiles: countFiles(srcPath, '.ts').toString(),
    htmlFiles: countFiles(srcPath, '.html').toString(),
    cssFiles: countFiles(srcPath, '.css').toString(),
    components: countFiles(appPath, '.component.ts').toString()
  };
  
  const timestamp = new Date().toISOString().split('T')[0];
  
  // Generate badges section
  const badges = `
## 📊 Project Statistics

![Components](https://img.shields.io/badge/Components-${stats.components}-blue)
![TypeScript](https://img.shields.io/badge/TypeScript-${stats.tsFiles}_files-blue)
![HTML](https://img.shields.io/badge/HTML-${stats.htmlFiles}_files-orange)
![CSS](https://img.shields.io/badge/CSS-${stats.cssFiles}_files-purple)
![Total Files](https://img.shields.io/badge/Total_Files-${stats.totalFiles}-green)

*Last updated: ${timestamp}*
`;
  
  // Update or insert badges
  if (content.includes('## 📊 Project Statistics')) {
    content = content.replace(
      /## 📊 Project Statistics[\s\S]*?(?=\n## |\n# |$)/,
      badges
    );
  } else {
    // Insert after main title
    const titleEnd = content.indexOf('\n\n');
    if (titleEnd !== -1) {
      content = content.slice(0, titleEnd) + '\n' + badges + content.slice(titleEnd);
    } else {
      content += '\n' + badges;
    }
  }
  
  fs.writeFileSync(readmePath, content);
  console.log('✅ README.md updated successfully');
  console.log(`   - Components: ${stats.components}`);
  console.log(`   - TypeScript files: ${stats.tsFiles}`);
  console.log(`   - HTML files: ${stats.htmlFiles}`);
  console.log(`   - CSS files: ${stats.cssFiles}`);
}

try {
  updateReadme();
} catch (error) {
  console.error('❌ Error updating README.md:', error.message);
  process.exit(1);
}
