import fs from 'fs';
import path from 'path';

function removeBOMAndTrim(filePath) {
  try {
    const content = fs.readFileSync(filePath, 'utf8');
    const trimmedContent = content.replace(/^\uFEFF/, '').trimStart();
    if (content !== trimmedContent) {
      fs.writeFileSync(filePath, trimmedContent, 'utf8');
      console.log(`Fixed: ${filePath}`);
    }
  } catch (error) {
    console.error(`Error processing ${filePath}:`, error);
  }
}

function processDirectory(directory) {
  const files = fs.readdirSync(directory);
  for (const file of files) {
    const filePath = path.join(directory, file);
    const stats = fs.statSync(filePath);
    if (stats.isDirectory()) {
      processDirectory(filePath);
    } else if (file.endsWith('.vue')) {
      removeBOMAndTrim(filePath);
    }
  }
}

// Start processing from the src directory
processDirectory('./src');
console.log('Processing complete!');
