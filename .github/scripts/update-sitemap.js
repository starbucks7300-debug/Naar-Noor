#!/usr/bin/env node

const fs = require('fs');
const path = require('path');

/**
 * Auto-update sitemap.xml with current routes and sections
 */

function extractSections() {
  const sections = [];
  const sectionsDir = path.join(process.cwd(), 'src', 'app', 'sections');
  
  if (!fs.existsSync(sectionsDir)) {
    return sections;
  }
  
  const items = fs.readdirSync(sectionsDir, { withFileTypes: true });
  
  items.forEach(item => {
    if (item.isDirectory()) {
      sections.push(item.name);
    }
  });
  
  return sections;
}

function generateSitemap() {
  const baseUrl = 'https://www.naarnooor.com';
  const today = new Date().toISOString().split('T')[0];
  
  // Extract sections
  const sections = extractSections();
  
  // Define main pages
  const pages = [
    { loc: '/', priority: '1.0', changefreq: 'weekly' },
    { loc: '/#about', priority: '0.8', changefreq: 'monthly' },
    { loc: '/#menu', priority: '0.9', changefreq: 'weekly' },
    { loc: '/#reservation', priority: '0.9', changefreq: 'monthly' },
    { loc: '/#chefs', priority: '0.7', changefreq: 'monthly' },
    { loc: '/#reviews', priority: '0.6', changefreq: 'weekly' },
    { loc: '/#blog', priority: '0.6', changefreq: 'weekly' },
    { loc: '/#locations', priority: '0.7', changefreq: 'monthly' }
  ];
  
  // Add discovered sections
  sections.forEach(section => {
    const sectionUrl = `/#${section}`;
    if (!pages.find(p => p.loc === sectionUrl)) {
      pages.push({
        loc: sectionUrl,
        priority: '0.6',
        changefreq: 'monthly'
      });
    }
  });
  
  // Generate XML
  let xml = '<?xml version="1.0" encoding="UTF-8"?>\n';
  xml += '<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">\n';
  
  pages.forEach(page => {
    xml += '  <url>\n';
    xml += `    <loc>${baseUrl}${page.loc}</loc>\n`;
    xml += `    <lastmod>${today}</lastmod>\n`;
    xml += `    <changefreq>${page.changefreq}</changefreq>\n`;
    xml += `    <priority>${page.priority}</priority>\n`;
    xml += '  </url>\n';
  });
  
  xml += '</urlset>\n';
  
  return xml;
}

function updateSitemap() {
  const sitemapPath = path.join(process.cwd(), 'src', 'sitemap.xml');
  
  const sitemap = generateSitemap();
  
  fs.writeFileSync(sitemapPath, sitemap);
  console.log('✅ sitemap.xml updated successfully');
  
  // Count URLs
  const urlCount = (sitemap.match(/<url>/g) || []).length;
  console.log(`   - Total URLs: ${urlCount}`);
}

try {
  updateSitemap();
} catch (error) {
  console.error('❌ Error updating sitemap.xml:', error.message);
  process.exit(1);
}
