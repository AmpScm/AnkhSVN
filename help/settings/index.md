---
title: AnkhSVN Settings
---

# AnkhSVN Settings

AnkhSVN settings combine Visual Studio integration choices with configuration used by the underlying Subversion client libraries.

When troubleshooting, identify whether a setting belongs to **AnkhSVN/Visual Studio**, **Subversion client configuration**, or the **repository/server**.

## Authentication

Subversion can cache authentication credentials and certificates according to client configuration.

If credentials changed but AnkhSVN keeps authenticating incorrectly:

- verify the repository URL,
- check whether cached credentials are stale,
- verify the account actually has access to the requested path,
- remember that externals may contact a different repository/server and require separate credentials.

Authentication success does not imply authorization to every repository path.

## HTTPS certificates

TLS/certificate failures can involve:

- an untrusted certificate authority,
- an expired certificate,
- a hostname mismatch,
- a corporate TLS-inspection proxy,
- a cached certificate decision.

Avoid permanently accepting an unexpected certificate until its identity has been verified.

## Proxy configuration

HTTP/HTTPS repository access may depend on Subversion proxy settings rather than Visual Studio's browser settings.

When access works in one SVN client but not another, compare:

- proxy host/port,
- bypass rules,
- credentials,
- protocol used by the repository URL,
- which Subversion configuration the client is reading.

## Diff and merge tools

AnkhSVN can invoke configured tools for comparing and merging files.

When configuring an external tool, verify:

- executable path,
- argument order,
- quoting of paths containing spaces,
- which argument represents mine/base/theirs/output,
- exit-code behavior when the tool supports it.

Test the tool with a disposable conflict before relying on it for important merges.

## Solution-specific behavior

Some AnkhSVN behavior is tied to the current solution rather than a machine-global setting. If behavior differs between two solutions on the same workstation, compare their working-copy layout and solution-specific source-control settings.

## Subversion configuration vs repository properties

Client configuration can provide machine-specific behavior such as global ignores or automatic properties.

Repository properties such as `svn:ignore`, `svn:externals`, and `svn:eol-style` are versioned and shared when committed.

Do not assume a behavior is shared with the team merely because it works on one workstation.

## External tools and environment

A tool launched by Visual Studio may see a different current directory, PATH, environment variables, or architecture than when you launch the same tool manually.

Use fully qualified executable paths when diagnosing tool-launch failures.

### Further reading

- [SVN Book: runtime configuration area](https://svnbook.red-bean.com/en/1.8/svn.advanced.confarea.html)
- [Subversion Properties](../properties/)
- [Troubleshooting](../troubleshooting/)

[Back to AnkhSVN Help](../)
