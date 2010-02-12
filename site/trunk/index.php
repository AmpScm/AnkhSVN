<?php
  require $_SERVER['DOCUMENT_ROOT'] . '/tpl/site.php';
  
  page_header('AnkhSVN - Subversion Support for Visual Studio');
?>
        <table id="welcome">
          <tr>
            <td id="welcome-img"><img src="ankhsvn/screenshots/welcome-small.png" alt="AnkhSVN in action" /></td>
            <td id="welcome-text">
            <div id="welcome-head">
              <strong>AnkhSVN</strong> is a Subversion SourceControl Provider
              for Microsoft Visual Studio 2005, 2008 and 2010.
            </div>
              
            <div id="welcome-body">
              <p>It provides Subversion source code management support to all
              projects and allows you to perform the most common
              version control operations directly from inside the Microsoft
              Visual Studio IDE.</p>
              
              <p>The <em>Pending Changes dashboard</em> gives you unique insight in
              all aspects of your development process and gives you easy access to
              the source code and issue management features. The deep sourcecode
              management integration in Visual Studio allows you to focus on
              developing with the tools you are accustomed to, while AnkhSVN tracks
              all changes for you and provides the tools to handle your
              specific needs.</p>
              
              <p>The Solution Explorer provides easy access to all Subversion
              management operations, but when you want to handle specific Subversion
              tasks you can use our Repository and Working copy explorers.</p>              
            </div>
            </td>
          </tr>
        </table>

<?php
  page_end();
                          