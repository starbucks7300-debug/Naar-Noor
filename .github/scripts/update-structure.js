#!/usr/bin/env node

const fs = require('fs');
const path = require('path');

/**
 * Auto-update STRUCTURE.md with current project structure
 */

function generateFileTree(dir, prefix = '', isLast = true) {
  const items = fs.readdirSync(dir, { withFileTypes: true })
    .filter(item => !item.name.startsWith('.') && item.name !== 'node_modules')
    .sort((a, b) => {
      if (a.isDirectory() && !b.isDirectory()) return -1;
      if (!a.isDirectory() && b.isDirectory()) return 1;
      return a.name.localeCompare(b.name);
    });

  let tree = '';
  
  items.forEach((item, index) => {
    const isLastItem = index === items.length - 1;
    const connector = isLastItem ? '└── ' : '├── ';
    const fullPath = path.join(dir, item.name);
    
    tree += `${prefix}${connector}${item.name}${item.isDirectory() ? '/' : ''}\n`;
    
    if (item.isDirectory() && !['node_modules', '.angular', 'dist'].includes(item.name)) {
      const newPrefix = prefix + (isLastItem ? '    ' : '│   ');
      tree += generateFileTree(fullPath, newPrefix, isLastItem);
    }
  });
  
  return tree;
}

function countFiles(dir, extension) {
  let count = 0;
  const items = fs.readdirSync(dir, { withFileTypes: true });
  
  items.forEach(item => {
    const fullPath = path.join(dir, item.name);
    if (item.isDirectory() && !['node_modules', '.angular', 'dist'].includes(item.name)) {
      count += countFiles(fullPath, extension);
    } else if (item.isFile() && item.name.endsWith(extension)) {
      count++;
    }
  });
  
  return count;
}

function updateStructureDoc() {
  const structurePath = path.join(process.cwd(), 'docs', 'STRUCTURE.md');
  
  if (!fs.existsSync(structurePath)) {
    console.log('STRUCTURE.md not found, skipping...');
    return;
  }
  
  let content = fs.readFileSync(structurePath, 'utf8');
  
  // Generate new file tree
  const tree = generateFileTree(process.cwd());
  
  // Count files
  const stats = {
    components: countFiles(path.join(process.cwd(), 'src', 'app'), '.component.ts'),
    typescript: countFiles(path.join(process.cwd(), 'src'), '.ts'),
    html: countFiles(path.join(process.cwd(), 'src'), '.html'),
    css: countFiles(path.join(process.cwd(), 'src'), '.css'),
  };
  
  // Update file tree section
  const treeRegex = /```\nlost-yeti-angular\/[\s\S]*?```/;
  if (treeRegex.test(content)) {
    content = content.replace(treeRegex, `\`\`\`\n${tree}\`\`\``);
  }
  
  // Add update timestamp
  const timestamp = new Date().toISOString().split('T')[0];
  const statsSection = `\n\n## Project Statistics\n\n` +
    `Last updated: ${timestamp}\n\n` +
    `- Total Components: ${stats.components}\n` +
    `- TypeScript Files: ${stats.typescript}\n` +
    `- HTML Templates: ${stats.html}\n` +
    `- CSS Stylesheets: ${stats.css}\n`;
  
  // Append or update stats section
  if (content.includes('## Project Statistics')) {
    content = content.replace(/## Project Statistics[\s\S]*$/, statsSection.trim());
  } else {
    content += statsSection;
  }
  
  fs.writeFileSync(structurePath, content);
  console.log('✅ STRUCTURE.md updated successfully');
  console.log(`   - Components: ${stats.components}`);
  console.log(`   - TypeScript: ${stats.typescript}`);
  console.log(`   - HTML: ${stats.html}`);
  console.log(`   - CSS: ${stats.css}`);
}

try {
  updateStructureDoc();
} catch (error) {
  console.error('❌ Error updating STRUCTURE.md:', error.message);
  process.exit(1);
}
