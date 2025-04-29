// eslint.config.mjs
import { Linter } from 'eslint';
import tsParser from '@typescript-eslint/parser';
import reactPlugin from 'eslint-plugin-react';
import reactHooksPlugin from 'eslint-plugin-react-hooks';

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
    plugins: {
      react: reactPlugin, // React plugin object
      'react-hooks': reactHooksPlugin, // React Hooks plugin object
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
      // React specific rules
      'react/jsx-uses-react': 'off', // React 17 JSX Transform no longer requires React import
      'react/react-in-jsx-scope': 'off', // React 17 JSX Transform no longer requires React in scope

      // React Hooks rules
      'react-hooks/rules-of-hooks': 'error', // Enforce rules of hooks
      'react-hooks/exhaustive-deps': 'warn', // Ensure all dependencies are in useEffect's dependency array
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
    plugins: {
      react: reactPlugin, // React plugin object
      'react-hooks': reactHooksPlugin, // React Hooks plugin object
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
      // React specific rules
      'react/jsx-uses-react': 'off', // React 17 JSX Transform no longer requires React import
      'react/react-in-jsx-scope': 'off', // React 17 JSX Transform no longer requires React in scope

      // React Hooks rules
      'react-hooks/rules-of-hooks': 'error', // Enforce rules of hooks
      'react-hooks/exhaustive-deps': 'warn', // Ensure all dependencies are in useEffect's dependency array
    },
  },
];
