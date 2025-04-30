## Pre-Push Hook

The `pre-push` hook runs **just before a push operation** is performed. It is executed when you run:

```bash
git push
```

This hook is ideal for performing checks that should be done **before changes are pushed to a remote repository**, such as running integration tests, preventing large files from being pushed, or ensuring that important checks (like security scans) are completed.

---

### 🔍 Purpose

- Prevent pushing commits that break the build or fail integration tests.
- Enforce code standards (e.g., prevent committing large files, secret keys, or sensitive information).
- Ensure that your push doesn't introduce errors that could affect other collaborators.
- Run checks for security vulnerabilities, like dependency scans.

---

### 🔧 How It Works

- Git executes the `pre-push` hook **just before** pushing commits to a remote.
- The hook will look for `.js` scripts inside `scripts/pre-push/`.
- Each `.js` file will be executed with Node.js.
- If any script exits with a non-zero code, the push is **canceled**.

---

### 📁 Directory Structure

```
scripts/
└── pre-push/
    ├── test-integration.js   # Run integration tests
    ├── file-size-check.js    # Prevent pushing large files
    └── security-scan.js      # Scan for sensitive information or vulnerabilities
```

---

### ✏️ Example Script: `test-integration.js`

```js
const { execSync } = require('child_process');

try {
  console.log('🔍 Running integration tests...');
  execSync('npm run test:integration', { stdio: 'inherit' });
} catch (err) {
  console.error('❌ Integration tests failed. Please fix the above errors.');
  process.exit(1);
}
```

---

### ✅ Recommended Checks

- **Run integration tests** to ensure that your code works with the latest changes.
- **Prevent large files** from being pushed using a file size checker.
- **Scan for sensitive data** (e.g., passwords, API keys) using a security scanner.
- **Enforce message formatting** for commit messages, if not already done by `commit-msg`.

---

### 🚀 Tip: Optimize Push Performance

- Keep `pre-push` scripts fast and avoid performing tasks that are more appropriate for continuous integration (CI), like running the entire test suite.
- If you need long-running checks, consider splitting them between `pre-commit` (for fast local validation) and `pre-push` (for integration-related tasks).

---