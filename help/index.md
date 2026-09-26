---
title: AnkhSVN Help
---

# AnkhSVN Help

AnkhSVN integrates Subversion working-copy and repository operations into Visual Studio. These pages explain both the AnkhSVN command you are using and the Subversion behavior behind it.

Help buttons include the dialog or control type in the URL so AnkhSVN can route you to the most relevant topic.

<p id="requested-topic"></p>

## Help topics

- [Annotate / Blame](annotate/)
- [Commit and Pending Changes](commit/)
- [Conflicts and Resolve](conflicts/)
- [Unified Diff](diff/)
- [Merge and merge tracking](merge/)
- [Repository Explorer and checkout](repository/)
- [Working-copy operations](working-copy/)
- [Externals](externals/)
- [Adding solutions and source-control setup](source-control/)
- [Subversion properties](properties/)
- [Issue tracking integration](issues/)
- [AnkhSVN settings, proxy, authentication, and external tools](settings/)
- [Troubleshooting](troubleshooting/)

## Subversion terms used in this help

**Working copy** is the local, editable copy of versioned files. **HEAD** means the youngest revision currently in the repository. **BASE** is the revision of an item that your working copy currently has recorded before local edits. A working copy can contain items at different BASE revisions.

Most AnkhSVN operations first modify the working copy. The repository does not change until a successful commit.

If a help button brought you here instead of a specific topic, the requested dialog name appears below. Include that name when opening a documentation issue.

<script>
(function () {
  var params = new URLSearchParams(window.location.search);
  var topic = params.get("dt") || "";
  var requested = document.getElementById("requested-topic");

  if (requested && topic) {
    requested.textContent = "Requested help topic: " + topic;
  }

  var name = topic.toLowerCase();
  var route = "";

  if (name.indexOf("annotate") >= 0 || name.indexOf("blame") >= 0) {
    route = "annotate/";
  } else if (name.indexOf("commonfileselectordialog") >= 0 || name.indexOf("unifieddiff") >= 0) {
    route = "diff/";
  } else if (name.indexOf("merge") >= 0) {
    route = "merge/";
  } else if (name.indexOf("issue") >= 0) {
    // PendingIssuesPage also contains "pendingchanges" in its full type name,
    // so issue help must win before the generic Pending Changes route.
    route = "issues/";
  } else if (name.indexOf("commit") >= 0 || name.indexOf("pendingchanges") >= 0 || name.indexOf("changelist") >= 0) {
    route = "commit/";
  } else if (name.indexOf("conflict") >= 0 || name.indexOf("resolve") >= 0) {
    route = "conflicts/";
  } else if (name.indexOf("external") >= 0) {
    route = "externals/";
  } else if (name.indexOf("checkout") >= 0 || name.indexOf("repository") >= 0) {
    route = "repository/";
  } else if (name.indexOf("update") >= 0 || name.indexOf("switch") >= 0 || name.indexOf("revert") >= 0 ||
             name.indexOf("lock") >= 0 || name.indexOf("workingcopy") >= 0 || name.indexOf("cleanup") >= 0) {
    route = "working-copy/";
  } else if (name.indexOf("sourcecontrol") >= 0 || name.indexOf("addtosubversion") >= 0 ||
             name.indexOf("solutionroot") >= 0 || name.indexOf(".scc.") >= 0 || name.indexOf("sccui") >= 0) {
    route = "source-control/";
  } else if (name.indexOf("property") >= 0) {
    route = "properties/";
  } else if (name.indexOf("proxy") >= 0 || name.indexOf("authentication") >= 0 || name.indexOf("tool") >= 0 ||
             name.indexOf("option") >= 0 || name.indexOf("setting") >= 0) {
    route = "settings/";
  } else if (name.indexOf("error") >= 0 || name.indexOf("warning") >= 0) {
    route = "troubleshooting/";
  }

  if (route) {
    var target = route;
    if (topic) {
      target += "?topic=" + encodeURIComponent(topic);
    }
    window.location.replace(target);
  }
})();
</script>
