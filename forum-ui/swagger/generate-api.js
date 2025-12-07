const fs = require('fs');
const path = require('path');

const swaggerFiles = [
  'combined.json'
];

const inputDir = path.resolve('swagger');
const outputDir = path.resolve('src/app/api');

if (!fs.existsSync(outputDir)) {
  fs.mkdirSync(outputDir, { recursive: true });
}

for (const file of swaggerFiles) {
  const input = path.join(inputDir, file);

  if (!fs.existsSync(input)) {
    console.warn('Swagger not found:', input);
    continue;
  }

  await generate({
    input,
    output: outputDir,
    httpClient: 'fetch',
    useOptions: true,
    useUnionTypes: true
  });

  console.log('Generated:', file);
}
