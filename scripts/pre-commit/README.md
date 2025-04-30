## 🛡️ Pre-Commit Hook

This hook runs **before a commit is finalized**, typically when you run:

```bash
git commit
```

It provides a final checkpoint to catch issues **before anything is committed** to the repository. Use this hook to enforce code quality, run linters, tests, or perform cleanup actions.

---

### 🔍 Purpose

- Prevent broken or improperly formatted code from being committed.
- Enforce consistent styling (e.g., Prettier, ESLint, Black).
- Run safety checks (e.g., prevent committing `.env` files, debug logs, or large binaries).
- Automate formatting and code cleanup.

---

### 🔧 How It Works

- Git executes the `pre-commit` hook script before opening or finalizing the commit.
- The hook looks for `.js` scripts inside `scripts/pre-commit/`.
- Each `.js` file is executed using Node.
- If any script exits with a non-zero code, the commit is **canceled**.

---

### 📁 Directory Structure

```
scripts/
└── pre-commit/
    ├── lint.js             # Run code linting
    ├── test.js             # Run basic tests
    └── prevent-debug.js    # Prevent accidental debug statements
```

---

### ✏️ Example Script: `lint.js`

```js
const { execSync } = require('child_process');

try {
  console.log('🔍 Running ESLint...');
  execSync('npx eslint .', { stdio: 'inherit' });
} catch (err) {
  console.error('❌ Linting failed. Please fix the above issues.');
  process.exit(1);
}
```
