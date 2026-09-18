---
title: Subversion Properties
---

# Subversion Properties

Subversion properties are versioned metadata attached to files or directories. Property edits behave much like file-content edits: they can be reviewed, committed, updated, reverted, and conflicted.

A property change is local until committed.

## Common Subversion properties

### svn:ignore

Defines unversioned names or patterns to ignore below a versioned directory.

Use it for generated files, build output, caches, or other files that should not normally be considered for version control.

An ignore rule does not remove an item that is already versioned.

### svn:externals

Defines additional working copies that Subversion should obtain below a versioned directory.

Externals are powerful enough to deserve their own topic: [Externals](../externals/).

### svn:eol-style

Controls line-ending normalization for text files. Common values include native platform line endings or a fixed line-ending style.

Changing this property can cause large apparent file changes if line endings are normalized.

### svn:keywords

Enables expansion of supported Subversion keywords in file content.

Be careful with generated or signed content where automatic expansion would be undesirable.

### svn:mime-type

Describes file content type. Subversion can use it to distinguish text-like and binary content behavior.

An incorrect binary MIME type can also affect operations such as Annotate.

### svn:needs-lock

Makes a file read-only in the working copy until a repository lock is obtained, as a reminder to use lock-modify-unlock behavior.

This is most useful for files that cannot be merged safely, such as many binary design formats.

### svn:executable

Marks a file as executable on platforms where executable permission is meaningful.

### svn:mergeinfo

Stores merge-tracking history.

Subversion normally maintains `svn:mergeinfo` automatically during merges. Manual edits should be avoided unless you understand how the merge history is being represented.

## Directory properties can affect descendants

Properties such as `svn:ignore` and `svn:externals` are commonly placed on directories. Some Subversion configurations also use inherited properties.

When debugging unexpected behavior, check parent directories as well as the immediate file.

## Property conflicts

Property changes can conflict independently from file content. A file may have no text conflict but still have an unresolved property conflict.

See [Conflicts and Resolve](../conflicts/).

## Automatic properties and global ignores

Some Subversion clients can apply properties automatically when files are added, based on client configuration. Global ignore settings can also hide files without using a versioned `svn:ignore` property.

That means two developers can see different add/ignore behavior if their local Subversion configuration differs.

### Further reading

- [SVN Book: properties](https://svnbook.red-bean.com/en/1.8/svn.advanced.props.html)
- [SVN Book: Subversion properties reference](https://svnbook.red-bean.com/en/1.8/svn.ref.properties.html)
- [Externals](../externals/)

[Back to AnkhSVN Help](../)
