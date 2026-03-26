#!/usr/bin/env node

const fs = require('fs');
const path = require('path');

/**
 * Auto-update TECHNOLOGIES.md with dependency versions
 */

function updateTechnologiesDoc() {
  const techPath = path.join(process.cwd(), 'docs', 'TECHNOLOGIES.md');
  const packagePath = path.join(process.cwd(), 'package.json');
  
  if (!fs.existsSync(packagePath)) {
    console.log('package.json not found, skipping...');
    return;
  }
  
  const packageJson = JSON.parse(fs.readFileSync(packagePath, 'utf8'));
  const dependencies = packageJson.dependencies || {};
  const devDependencies = packageJson.devDependencies || {};
  
  // Create or update TECHNOLOGIES.md
  const timestamp = new Date().toISOString().split('T')[0];
  
  let content = `# Technology Stack\n\n`;
  content += `Last updated: ${timestamp}\n\n`;
  content += `## Overview\n\n`;
  content += `The Lost Yeti Kitchen & Bar is built with modern web technologies focused on performance, maintainability, and user experience.\n\n`;
  
  // Core Framework
  content += `## Core Framework\n\n`;
  content += `### Angular\n`;
  content += `- **Version**: ${dependencies['@angular/core'] || 'N/A'}\n`;
  content += `- **Architecture**: Standalone Components\n`;
  content += `- **Language**: TypeScript ${devDependencies['typescript'] || 'N/A'}\n`;
  content += `- **Build Tool**: Angular CLI ${devDependencies['@angular/cli'] || 'N/A'}\n\n`;
  
  // Frontend Dependencies
  content += `## Frontend Dependencies\n\n`;
  content += `### UI & Styling\n`;
  content += `- **Tailwind CSS**: CDN (latest)\n`;
  content += `- **Google Fonts**: Forum, Open Sans\n`;
  content += `- **Icons**: Iconify\n`;
  content += `- **3D Graphics**: Spline Design\n\n`;
  
  // Angular Packages
  content += `### Angular Packages\n\n`;
  const angularPackages = Object.keys(dependencies)
    .filter(pkg => pkg.startsWith('@angular/'))
    .sort();
  
  angularPackages.forEach(pkg => {
    content += `- **${pkg}**: ${dependencies[pkg]}\n`;
  });
  content += `\n`;
  
  // Other Dependencies
  content += `### Other Dependencies\n\n`;
  const otherDeps = Object.keys(dependencies)
    .filter(pkg => !pkg.startsWith('@angular/'))
    .sort();
  
  otherDeps.forEach(pkg => {
    content += `- **${pkg}**: ${dependencies[pkg]}\n`;
  });
  content += `\n`;
  
  // Development Dependencies
  content += `## Development Dependencies\n\n`;
  const devDeps = Object.keys(devDependencies).sort();
  
  devDeps.forEach(pkg => {
    content += `- **${pkg}**: ${devDependencies[pkg]}\n`;
  });
  content += `\n`;
  
  // Build & Deployment
  content += `## Build & Deployment\n\n`;
  content += `### Build Configuration\n`;
  content += `- **Production Build**: \`npm run build:prod\`\n`;
  content += `- **Development Build**: \`npm run build\`\n`;
  content += `- **Development Server**: \`npm start\`\n\n`;
  
  content += `### Optimization Features\n`;
  content += `- Ahead-of-Time (AOT) Compilation\n`;
  content += `- Tree Shaking\n`;
  content += `- Code Splitting\n`;
  content += `- Minification\n`;
  content += `- Source Maps (development only)\n`;
  content += `- Bundle Size Budgets\n\n`;
  
  // Browser Support
  content += `## Browser Support\n\n`;
  content += `- Chrome (latest)\n`;
  content += `- Firefox (latest)\n`;
  content += `- Safari (latest)\n`;
  content += `- Edge (latest)\n`;
  content += `- Mobile browsers (iOS Safari, Chrome Mobile)\n\n`;
  
  // Performance
  content += `## Performance Features\n\n`;
  content += `- DNS Prefetch for external resources\n`;
  content += `- Deferred script loading\n`;
  content += `- Lazy loading (route-based)\n`;
  content += `- Image optimization\n`;
  content += `- CSS minification\n`;
  content += `- JavaScript minification\n\n`;
  
  // SEO
  content += `## SEO & Meta\n\n`;
  content += `- Open Graph tags\n`;
  content += `- Twitter Card tags\n`;
  content += `- Schema.org structured data\n`;
  content += `- Sitemap.xml\n`;
  content += `- Robots.txt\n`;
  content += `- Canonical URLs\n\n`;
  
  // Version Info
  content += `## Version Information\n\n`;
  content += `- **Project Version**: ${packageJson.version || '1.0.0'}\n`;
  content += `- **Node.js**: >=18.0.0 (recommended: 20.x)\n`;
  content += `- **npm**: >=9.0.0\n\n`;
  
  fs.writeFileSync(techPath, content);
  console.log('✅ TECHNOLOGIES.md updated successfully');
  console.log(`   - Dependencies: ${Object.keys(dependencies).length}`);
  console.log(`   - Dev Dependencies: ${Object.keys(devDependencies).length}`);
}

try {
  updateTechnologiesDoc();
} catch (error) {
  console.error('❌ Error updating TECHNOLOGIES.md:', error.message);
  process.exit(1);
}
