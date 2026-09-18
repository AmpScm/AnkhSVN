---
title: AnkhSVN Help
---

# AnkhSVN Help

This is the built-in help landing page for AnkhSVN. Help buttons in the Visual Studio extension open this page and include the dialog or control type so the documentation can route to the most relevant topic.

<p id="requested-topic"></p>

## Help topics

- [Merge and merge tracking](merge/)
- [Commit and Pending Changes](commit/)
- [Repository Explorer and checkout](repository/)
- [Update, switch, revert, lock, and working-copy operations](working-copy/)
- [Adding solutions and source-control setup](source-control/)
- [Subversion properties](properties/)
- [Issue tracking integration](issues/)
- [AnkhSVN settings, proxy, authentication, and external tools](settings/)
- [Troubleshooting](troubleshooting/)

If a help button brought you here instead of a specific topic, the dialog name shown below can be included in a GitHub issue so the documentation can be expanded.

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

  if (name.indexOf("merge") >= 0) {
    route = "merge/";
  } else if (name.indexOf("commit") >= 0 || name.indexOf("pendingchanges") >= 0 || name.indexOf("changelist") >= 0) {
    route = "commit/";
  } else if (name.indexOf("checkout") >= 0 || name.indexOf("repository") >= 0) {
    route = "repository/";
  } else if (name.indexOf("update") >= 0 || name.indexOf("switch") >= 0 || name.indexOf("revert") >= 0 ||
             name.indexOf("lock") >= 0 || name.indexOf("workingcopy") >= 0 || name.indexOf("cleanup") >= 0 ||
             name.indexOf("resolve") >= 0) {
    route = "working-copy/";
  } else if (name.indexOf("sourcecontrol") >= 0 || name.indexOf("addtosubversion") >= 0 ||
             name.indexOf("solutionroot") >= 0 || name.indexOf(".scc.") >= 0 || name.indexOf("sccui") >= 0) {
    route = "source-control/";
  } else if (name.indexOf("property") >= 0) {
    route = "properties/";
  } else if (name.indexOf("issue") >= 0) {
    route = "issues/";
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
