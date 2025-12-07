const fs = require('fs');
const path = require('path');

function readJsonFile(filepath) {
  const data = fs.readFileSync(filepath, 'utf8');
  return JSON.parse(data);
}

function writeJsonFile(filepath, data) {
  fs.writeFileSync(filepath, JSON.stringify(data, null, 2), 'utf8');
}

const config = {
  apis: [
    'identity.swagger.json',
  ]
};

const configPath = path.join(__dirname, 'swagger-combine-config.json');
if (fs.existsSync(configPath)) {
  const fileConfig = readJsonFile(configPath);
  if (fileConfig.apis) {
    config.apis = fileConfig.apis;
  }
}

let result = {
  openapi: "3.0.1",
  info: {
    title: "Forum API",
    version: "1.0.0",
    description: "Combined API for Forum project"
  },
  servers: [
    {
      url: "http://localhost:5000",
      description: "Local development server"
    }
  ],
  paths: {},
  components: {
    schemas: {},
    securitySchemes: {
      bearer: {
        type: "http",
        scheme: "bearer",
        bearerFormat: "JWT"
      }
    }
  },
  security: [
    {
      bearer: []
    }
  ]
};

config.apis.forEach(apiPath => {
  const fullPath = path.isAbsolute(apiPath) ? apiPath : path.join(__dirname, apiPath);

  if (!fs.existsSync(fullPath)) {
    console.warn(`Swagger file not found: ${fullPath}`);
    return;
  }

  try {
    const api = readJsonFile(fullPath);

    // Объединяем paths
    if (api.paths) {
      result.paths = { ...api.paths, ...result.paths };
    }

    // Объединяем schemas
    if (api.components && api.components.schemas) {
      result.components.schemas = {
        ...api.components.schemas,
        ...result.components.schemas
      };
    }

    // Объединяем security schemes если есть
    if (api.components && api.components.securitySchemes) {
      result.components.securitySchemes = {
        ...result.components.securitySchemes,
        ...api.components.securitySchemes
      };
    }

    console.log(`Added API: ${apiPath}`);
  } catch (error) {
    console.error(`Error processing ${apiPath}:`, error.message);
  }
});

writeJsonFile(
  path.join(__dirname, 'swagger-combine-result.json'),
  result
);

console.log('Swagger files combined successfully!');
