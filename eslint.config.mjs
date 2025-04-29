// eslint.config.mjs
import { Linter } from 'eslint';
import tsParser from '@typescript-eslint/parser';

/** @type {Linter.FlatConfig[]} */
export default [
  {
    // For JavaScript and JSX files
    files: ['**/*.js', '**/*.jsx'],
    languageOptions: {
      parser: tsParser, // Use the default parser for JavaScript
      parserOptions: {
        ecmaVersion: 2021, // Use ECMAScript 2021 features
        sourceType: 'module', // Support ES modules
        ecmaFeatures: {
          jsx: true, // Enable JSX parsing
        },
      },
    },
    rules: {
      'no-restricted-imports': [
        'error',
        {
          paths: [
            {
              name: 'UI/Functions/WebRequest',
              importNames: [
                'default',
                'webRequest',
                'getBlob',
                'getTextResponse',
                'getText',
                'getJson',
                'getList',
                'getOne',
              ],
              message: "Do not import from 'UI/Functions/WebRequest' directly. Use the generated API in the 'TypeScript' directory instead.",
            },
          ],
        },
      ],
    },
  },
  {
    // For TypeScript and TSX files
    files: ['**/*.ts', '**/*.tsx'],
    languageOptions: {
      parser: tsParser, // Use @typescript-eslint/parser for TypeScript files
      parserOptions: {
        ecmaVersion: 2021, // Use ECMAScript 2021 features
        sourceType: 'module', // Support ES modules
        project: './tsconfig.json', // Specify tsconfig.json for type checking
      },
    },
    rules: {
      'no-restricted-imports': [
        'error',
        {
          paths: [
            {
              name: 'UI/Functions/WebRequest',
              importNames: [
                'default',
                'webRequest',
                'getBlob',
                'getTextResponse',
                'getText',
                'getJson',
                'getList',
                'getOne',
              ],
              message: "Do not import from 'UI/Functions/WebRequest' directly. Use the generated API in the 'typescript' directory instead.",
            },
          ],
        },
      ],
    },
  },
];
