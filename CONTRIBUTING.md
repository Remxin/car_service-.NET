# 🧩 Repo Guidelines

This file includes basic naming and commit conventions we stick to in the project. It's just for internal use, but following this keeps our workflow clean and easy to follow.

---

## 🌿 Branch Naming

Use the following naming pattern for branches:
```aiignore
<type>/<short-description>
```


### Types:
- `feature` – for new features or components
- `bugfix` – for bug fixes
- `hotfix` – for urgent production fixes
- `refactor` – for code restructuring with no behavior change
- `docs` – for documentation updates
- `chore` – for tasks like dependency updates, tooling, CI/CD etc.

### Examples:
- `feature/user-auth`
- `bugfix/form-validation`
- `docs/setup-instructions`

---

## 📝 Commit Messages

We follow a simplified version of [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/). Stick to this format:


### Types:
- `feature` – for new features or components
- `bugfix` – for bug fixes
- `hotfix` – for urgent production fixes
- `refactor` – for code restructuring with no behavior change
- `docs` – for documentation updates
- `chore` – for tasks like dependency updates, tooling, CI/CD etc.

### Examples:
- `feature/user-auth`
- `bugfix/form-validation`
- `docs/setup-instructions`

---

## 📝 Commit Messages

We follow a simplified version of [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/). Stick to this format:


### Commit types:
- `feat` – a new feature
- `fix` – a bug fix
- `docs` – docs only changes
- `refactor` – refactoring code
- `style` – formatting only (whitespace, commas, etc.)
- `test` – adding or updating tests
- `chore` – other tasks (build scripts, tooling, etc.)

### Examples:
- `feat(login): add login form`
- `fix(api): resolve 500 error on data fetch`
- `docs: update README`
- `refactor: simplify validation logic`

---

## 🔄 Basic Workflow

1. `git checkout -b feature/task-name`
2. Make your changes
3. `git commit -m "feat(...): message"`
4. `git push origin feature/task-name`
5. Open a Pull Request to `main` or `dev` (based on team flow)

---

Thanks! Clean code and commit history = future us will thank us 🙌
