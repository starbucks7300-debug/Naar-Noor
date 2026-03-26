#!/usr/bin/env node

const fs = require('fs');
const path = require('path');

/**
 * Auto-update README.md with project statistics
 */

function updateReadme() {
  const readmePath = path.join(process.cwd(), 'README.md');
  
  if (!fs.existsSync(readmePath)) {
    console.log('README.md not found, skipping...');
    return;
  }
  
  let content = fs.readFileSync(readmePath, 'utf8');
  
  // Get stats from environment
  const stats = {
    totalFiles: process.env.TOTAL_FILES || '0',
    tsFiles: process.env.TS_FILES || '0',
    htmlFiles: process.env.HTML_FILES || '0',
    cssFiles: process.env.CSS_FILES || '0',
    components: process.env.COMPONENTS || '0'
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
