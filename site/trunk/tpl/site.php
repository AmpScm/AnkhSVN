<?php

$tpl_state = 0;

function page_start($title)
{
  if ($tpl_state >= 10)
    return;
  $tpl_state = 10;
header('Content-Type: text/html; charset=Utf-8');
header('X-UA-Compatible: IE=8');
?>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
  <head>
    <title><?=htmlentities($title)?></title>
    <link rel="stylesheet" type="text/css" href="/styles/site.css">
    <link rel="icon" type="image/vnd.microsoft.icon" href="/favicon.ico"> 
    
<?php
}

function page_header($title)
{
  page_start($title);
  
  if ($tpl_state >= 20)
    return;
  $tpl_state = 20;
?>
  </head>
  <body>
<?php
  include $_SERVER['DOCUMENT_ROOT'] . '/tpl/page_start.php';
}

function page_end()
{
?>
    </div>
  </body>
</html>
<?php
}