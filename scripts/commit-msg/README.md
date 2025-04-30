## 📦 Commit Message Hook

This hook runs automatically whenever you execute:

```bash
git commit -m "Your message here"
```

It is used to **enforce commit message style guidelines**, such as requiring specific prefixes, lengths, or formats (e.g., Conventional Commits).

### 🔍 Purpose

- Enforce a consistent commit message style across the team.
- Prevent commits with empty, vague, or malformed messages.
- Support automation workflows (e.g., changelog generation, semantic versioning).

### 🔧 How It Works

- Git runs the `commit-msg` hook before completing a commit.
- It passes the commit message file (a temporary file containing your message) to each script in `scripts/commit-msg/`.
- Each `.js` script receives the commit message file path as an argument.
- If any script fails (exits with a non-zero code), the commit is **blocked**.

### 📁 Directory Structure

```
scripts/
└── commit-msg/
    ├── validate-format.js   # Example validator script
    └── check-empty.js       # Check for empty commit messages
```

### ✏️ Example Script: `check-empty.js`

```js
const fs = require('fs');
const message = fs.readFileSync(process.argv[2], 'utf8').trim();

if (!message) {
  console.error('❌ Commit message is empty. Please provide a meaningful message.');
  process.exit(1);
}
```

### ✅ Recommended Rules

You can enforce rules like:

- Commit messages must start with a type: `feat:`, `fix:`, `docs:`, etc.
- Must be under 72 characters on the first line.
- Must not use generic messages like "fix", "changes", etc.