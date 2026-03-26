#!/usr/bin/env node

const fs = require('fs');
const path = require('path');

/**
 * Auto-update FEATURES.md with component information
 */

function extractComponentInfo(filePath) {
  const content = fs.readFileSync(filePath, 'utf8');
  
  // Extract selector
  const selectorMatch = content.match(/selector:\s*['"]([^'"]+)['"]/);
  const selector = selectorMatch ? selectorMatch[1] : null;
  
  // Extract class name
  const classMatch = content.match(/export\s+class\s+(\w+)/);
  const className = classMatch ? classMatch[1] : null;
  
  // Extract inputs
  const inputs = [];
  const inputRegex = /@Input\(\)\s+(\w+):\s*([^;]+);/g;
  let inputMatch;
  while ((inputMatch = inputRegex.exec(content)) !== null) {
    inputs.push({ name: inputMatch[1], type: inputMatch[2].trim() });
  }
  
  // Extract outputs
  const outputs = [];
  const outputRegex = /@Output\(\)\s+(\w+)\s*=\s*new\s+EventEmitter<([^>]+)>/g;
  let outputMatch;
  while ((outputMatch = outputRegex.exec(content)) !== null) {
    outputs.push({ name: outputMatch[1], type: outputMatch[2].trim() });
  }
  
  return {
    selector,
    className,
    inputs,
    outputs,
    path: filePath
  };
}

function scanComponents(dir) {
  const components = [];
  
  function scan(directory) {
    const items = fs.readdirSync(directory, { withFileTypes: true });
    
    items.forEach(item => {
      const fullPath = path.join(directory, item.name);
      
      if (item.isDirectory()) {
        scan(fullPath);
      } else if (item.name.endsWith('.component.ts')) {
        try {
          const info = extractComponentInfo(fullPath);
          if (info.selector && info.className) {
            components.push(info);
          }
        } catch (error) {
          console.warn(`Warning: Could not parse ${fullPath}`);
        }
      }
    });
  }
  
  scan(dir);
  return components;
}

function updateFeaturesDoc(componentCount, sectionCount) {
  const featuresPath = path.join(process.cwd(), 'docs', 'FEATURES.md');
  
  if (!fs.existsSync(featuresPath)) {
    console.log('FEATURES.md not found, skipping...');
    return;
  }
  
  let content = fs.readFileSync(featuresPath, 'utf8');
  
  // Scan for components
  const srcPath = path.join(process.cwd(), 'src', 'app');
  const components = scanComponents(srcPath);
  
  // Generate component list
  const timestamp = new Date().toISOString().split('T')[0];
  const componentList = `\n\n## Component Inventory\n\n` +
    `Last updated: ${timestamp}\n\n` +
    `Total Components: ${components.length}\n\n` +
    `### Components List\n\n` +
    components.map(comp => {
      let info = `#### ${comp.className}\n\n`;
      info += `- **Selector**: \`${comp.selector}\`\n`;
      info += `- **Location**: \`${comp.path.replace(process.cwd(), '')}\`\n`;
      
      if (comp.inputs.length > 0) {
        info += `- **Inputs**:\n`;
        comp.inputs.forEach(input => {
          info += `  - \`${input.name}: ${input.type}\`\n`;
        });
      }
      
      if (comp.outputs.length > 0) {
        info += `- **Outputs**:\n`;
        comp.outputs.forEach(output => {
          info += `  - \`${output.name}: EventEmitter<${output.type}>\`\n`;
        });
      }
      
      return info;
    }).join('\n');
  
  // Append or update component inventory
  if (content.includes('## Component Inventory')) {
    content = content.replace(/## Component Inventory[\s\S]*$/, componentList.trim());
  } else {
    content += componentList;
  }
  
  fs.writeFileSync(featuresPath, content);
  console.log('✅ FEATURES.md updated successfully');
  console.log(`   - Total components: ${components.length}`);
}

try {
  const componentCount = process.argv[2] || 0;
  const sectionCount = process.argv[3] || 0;
  updateFeaturesDoc(componentCount, sectionCount);
} catch (error) {
  console.error('❌ Error updating FEATURES.md:', error.message);
  process.exit(1);
}
