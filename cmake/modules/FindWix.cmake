

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en">
<head>
  <title>
  sinbad / ogre / source &mdash; Bitbucket
</title>
  <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
  <meta name="description" content="" />
  <meta name="keywords" content="" />
  
  <!--[if lt IE 9]>
  <script src="https://dwz7u9t8u8usb.cloudfront.net/m/fbe390655bc5/js/old/html5.js"></script>
  <![endif]-->

  <script>
    (function (window) {
      // prevent stray occurrences of `console.log` from causing errors in IE
      var console = window.console || (window.console = {});
      console.log || (console.log = function () {});

      var BB = window.BB || (window.BB = {});
      BB.debug = false;
      BB.cname = false;
      BB.CANON_URL = 'https://bitbucket.org';
      BB.MEDIA_URL = 'https://dwz7u9t8u8usb.cloudfront.net/m/fbe390655bc5/';
      BB.images = {
        invitation: 'https://dwz7u9t8u8usb.cloudfront.net/m/fbe390655bc5/img/icons/fugue/card_address.png',
        noAvatar: 'https://dwz7u9t8u8usb.cloudfront.net/m/fbe390655bc5/img/no_avatar.png'
      };
      BB.user = {"isKbdShortcutsEnabled": true, "isSshEnabled": false};
      BB.user.has = (function () {
        var betaFeatures = [];
        betaFeatures.push('repo2');
        return function (feature) {
          return _.contains(betaFeatures, feature);
        };
      }());
      BB.targetUser = BB.user;
  
    
  
      BB.repo || (BB.repo = {});
  
      
        BB.user.repoPrivilege = null;
      
      
        
          BB.user.accountPrivilege = null;
        
      
      BB.repo.id = 49295;
    
    
      BB.repo.language = 'c++';
      BB.repo.pygmentsLanguage = 'c++';
    
    
      BB.repo.slug = 'ogre';
    
    
      BB.repo.owner = {"username": "sinbad", "displayName": "Steve Streeting", "firstName": "Steve", "avatarUrl": "https://secure.gravatar.com/avatar/fbe8cc9ac5bc8797382e01e10f5f8e33?d=identicon\u0026s=32", "follows": {"repos": [49295, 80313, 83849, 163588, 193912, 243379, 251810, 267522, 274331, 286412, 354613, 362417, 443375, 443404, 608661, 608701, 613393, 730091, 953506, 960822, 970506, 1067945, 1095009]}, "isTeam": false, "isSshEnabled": true, "lastName": "Streeting", "isKbdShortcutsEnabled": true, "id": 18404};
    
    
      
        
      
    
    
      // Coerce `BB.repo` to a string to get
      // "davidchambers/mango" or whatever.
      BB.repo.toString = function () {
        return BB.cname ? this.slug : '{owner.username}/{slug}'.format(this);
      }
    
    
      BB.changeset = 'c28c82519c70'
    
    
  
    }(this));
  </script>

  


  <link rel="stylesheet" href="https://dwz7u9t8u8usb.cloudfront.net/m/fbe390655bc5/bun/css/bundle.css"/>



  <link rel="search" type="application/opensearchdescription+xml" href="/opensearch.xml" title="Bitbucket" />
  <link rel="icon" href="https://dwz7u9t8u8usb.cloudfront.net/m/fbe390655bc5/img/logo_new.png" type="image/png" />
  <link type="text/plain" rel="author" href="/humans.txt" />


  
  
    <script src="https://dwz7u9t8u8usb.cloudfront.net/m/fbe390655bc5/bun/js/bundle.js"></script>
  



</head>

<body id="" class=" ">
  <script>
    if (navigator.userAgent.indexOf(' AppleWebKit/') === -1) {
      $('body').addClass('non-webkit')
    }
    $('body')
      .addClass($.client.os.toLowerCase())
      .addClass($.client.browser.toLowerCase())
  </script>
  <!--[if IE 8]>
  <script>jQuery(document.body).addClass('ie8')</script>
  <![endif]-->
  <!--[if IE 9]>
  <script>jQuery(document.body).addClass('ie9')</script>
  <![endif]-->

  <div id="wrapper">



  <div id="header-wrap">
    <div id="header">
    <ul id="global-nav">
      <li><a class="home" href="http://www.atlassian.com">Atlassian Home</a></li>
      <li><a class="docs" href="http://confluence.atlassian.com/display/BITBUCKET">Documentation</a></li>
      <li><a class="support" href="/support">Support</a></li>
      <li><a class="blog" href="http://blog.bitbucket.org">Blog</a></li>
      <li><a class="forums" href="http://groups.google.com/group/bitbucket-users">Forums</a></li>
    </ul>
    <a href="/" id="logo">Bitbucket by Atlassian</a>

    <div id="main-nav">
    
      <ul class="clearfix">
        <li><a href="/plans">Pricing &amp; signup</a></li>
        <li><a id="explore-link" href="/explore">Explore Bitbucket</a></li>
        <li><a href="/account/signin/?next=/sinbad/ogre/src/c28c82519c70/CMake/Packages/FindWix.cmake">Log in</a></li>
        

<li class="search-box">
  
    <form action="/repo/all">
      <input type="search" results="5" autosave="bitbucket-explore-search"
             name="name" id="searchbox"
             placeholder="owner/repo" />
  
  </form>
</li>

      </ul>
    
    </div>

  

    </div>
  </div>

    <div id="header-messages">
  
    
    
    
    
  

    
   </div>



    <div id="content">
      <div id="source">
      
  
  





  <script>
    jQuery(function ($) {
        var cookie = $.cookie,
            cookieOptions, date,
            $content = $('#content'),
            $pane = $('#what-is-bitbucket'),
            $hide = $pane.find('[href="#hide"]').css('display', 'block').hide();

        date = new Date();
        date.setTime(date.getTime() + 365 * 24 * 60 * 60 * 1000);
        cookieOptions = { path: '/', expires: date };

        if (cookie('toggle_status') == 'hide') $content.addClass('repo-desc-hidden');

        $('#toggle-repo-content').click(function (event) {
            event.preventDefault();
            $content.toggleClass('repo-desc-hidden');
            cookie('toggle_status', cookie('toggle_status') == 'show' ? 'hide' : 'show', cookieOptions);
        });

        if (!cookie('hide_intro_message')) $pane.show();

        $hide.click(function (event) {
            event.preventDefault();
            cookie('hide_intro_message', true, cookieOptions);
            $pane.slideUp('slow');
        });

        $pane.hover(
            function () { $hide.fadeIn('fast'); },
            function () { $hide.fadeOut('fast'); });

      (function () {
        // Update "recently-viewed-repos" cookie for
        // the "repositories" drop-down.
        var
          id = BB.repo.id,
          cookieName = 'recently-viewed-repos_' + BB.user.id,
          rvr = cookie(cookieName),
          ids = rvr? rvr.split(','): [],
          idx = _.indexOf(ids, '' + id);

        // Remove `id` from `ids` if present.
        if (~idx) ids.splice(idx, 1);

        cookie(
          cookieName,
          // Insert `id` as the first item, then call
          // `join` on the resulting array to produce
          // something like "114694,27542,89002,84570".
          [id].concat(ids.slice(0, 4)).join(),
          {path: '/', expires: 1e6} // "never" expires
        );
      }());
    });
  </script>



  <meta name="twitter:card" value="summary"/>
  <meta name="twitter:site" value="@bitbucket"/>
  <meta name="twitter:url" value="/sinbad/ogre"/>
  <meta name="twitter:title" value="sinbad/ogre - bitbucket.org"/>
  <meta name="twitter:description" value="Official Mercurial repository for OGRE"/>



    <div id="what-is-bitbucket" class="new-to-bitbucket">
      <h2>Steve Streeting <span id="slogan">is sharing code with you</span></h2>
      <img src="https://secure.gravatar.com/avatar/fbe8cc9ac5bc8797382e01e10f5f8e33?d=identicon&amp;s=32" alt="" class="avatar" />
      <p>Bitbucket is a code hosting site. Unlimited public and private repositories. Free for small teams.</p>
      <div class="primary-action-link signup"><a href="/account/signup/?utm_source=internal&utm_medium=banner&utm_campaign=what_is_bitbucket">Try Bitbucket free</a></div>
      <a href="#hide" title="Don't show this again">Don't show this again</a>
    </div>


<div id="tabs" class="tabs">
  <ul>
    
      <li>
        <a href="/sinbad/ogre/overview" id="repo-overview-link">Overview</a>
      </li>
    

    
      <li>
        <a href="/sinbad/ogre/downloads" id="repo-downloads-link">Downloads (<span id="downloads-count">0</span>)</a>
      </li>
    

    
      
    

    
      <li>
        <a href="/sinbad/ogre/pull-requests" id="repo-pr-link">Pull requests (1)</a>
      </li>
    

    
      <li class="selected">
        
          <a href="/sinbad/ogre/src" id="repo-source-link">Source</a>
        
      </li>
    

    
      <li>
        <a href="/sinbad/ogre/changesets" id="repo-commits-link">Commits</a>
      </li>
    

    <li id="wiki-tab" class="dropdown"
      style="display:
                        none  
        
      ">
      <a href="/sinbad/ogre/wiki" id="repo-wiki-link">Wiki</a>
    </li>

    <li id="issues-tab" class="dropdown inertial-hover"
      style="display:
                      none  
        
      ">
      <a href="/sinbad/ogre/issues?status=new&amp;status=open" id="repo-issues-link">Issues (0) &raquo;</a>
      <ul>
        <li><a href="/sinbad/ogre/issues/new">Create new issue</a></li>
        <li><a href="/sinbad/ogre/issues?status=new">New issues</a></li>
        <li><a href="/sinbad/ogre/issues?status=new&amp;status=open">Open issues</a></li>
        <li><a href="/sinbad/ogre/issues?status=duplicate&amp;status=invalid&amp;status=resolved&amp;status=wontfix">Closed issues</a></li>
        
        <li><a href="/sinbad/ogre/issues">All issues</a></li>
        <li><a href="/sinbad/ogre/issues/query">Advanced query</a></li>
      </ul>
    </li>

    
  </ul>

  <ul>
    
      <li>
        <a href="/sinbad/ogre/descendants" id="repo-forks-link">Forks/queues (67)</a>
      </li>
    

    
      <li>
        <a href="/sinbad/ogre/zealots">Followers (<span id="followers-count">232</span>)</a>
      </li>
    
  </ul>
</div>



 


  <div class="repo-menu" id="repo-menu">
    <ul id="repo-menu-links">
    
    
      <li>
        <a href="/sinbad/ogre/rss" class="rss" title="RSS feed for ogre">RSS</a>
      </li>

      <li><a id="repo-fork-link" href="/sinbad/ogre/fork" class="fork">fork</a></li>
      
        
          <li><a id="repo-patch-queue-link" href="/sinbad/ogre/hack" class="patch-queue">patch queue</a></li>
        
      
      <li>
        <a id="repo-follow-link" rel="nofollow" href="/sinbad/ogre/follow" class="follow">follow</a>
      </li>
      
          
      
      
        <li class="get-source inertial-hover">
          <a class="source">get source</a>
          <ul class="downloads">
            
              
              <li><a rel="nofollow" href="/sinbad/ogre/get/c28c82519c70.zip">zip</a></li>
              <li><a rel="nofollow" href="/sinbad/ogre/get/c28c82519c70.tar.gz">gz</a></li>
              <li><a rel="nofollow" href="/sinbad/ogre/get/c28c82519c70.tar.bz2">bz2</a></li>
            
          </ul>
        </li>
      
      
    </ul>

  
    <ul class="metadata">
      
      
      
        <li class="branches inertial-hover">branches
          <ul>
            <li class="filter">
              <input type="text" class="dropdown-filter" placeholder="Filter branches" autosave="branch-dropdown-49295"/>
            </li>
            
            <li class="comprev"><a href="/sinbad/ogre/src/c28c82519c70" title="default">default</a>
              
            </li>
            <li class="comprev"><a href="/sinbad/ogre/src/ad6be123a04c" title="v1-6">v1-6</a>
              
              <a rel="nofollow" class="menu-compare"
                 href="/sinbad/ogre/compare/v1-6..default"
                 title="Show changes between v1-6 and the main branch.">compare</a>
              
            </li>
            <li class="comprev"><a href="/sinbad/ogre/src/72308319f630" title="v1-7">v1-7</a>
              
              <a rel="nofollow" class="menu-compare"
                 href="/sinbad/ogre/compare/v1-7..default"
                 title="Show changes between v1-7 and the main branch.">compare</a>
              
            </li>
            <li class="comprev"><a href="/sinbad/ogre/src/f038953bf2e9" title="v1-8">v1-8</a>
              
              <a rel="nofollow" class="menu-compare"
                 href="/sinbad/ogre/compare/v1-8..default"
                 title="Show changes between v1-8 and the main branch.">compare</a>
              
            </li>
            <li class="comprev"><a href="/sinbad/ogre/src/547ffd4a5b5f" title="v1-9">v1-9</a>
              
              <a rel="nofollow" class="menu-compare"
                 href="/sinbad/ogre/compare/v1-9..default"
                 title="Show changes between v1-9 and the main branch.">compare</a>
              
            </li>
            <li class="comprev"><a href="/sinbad/ogre/src/4f48da6e190b" title="v2-0">v2-0</a>
              
              <a rel="nofollow" class="menu-compare"
                 href="/sinbad/ogre/compare/v2-0..default"
                 title="Show changes between v2-0 and the main branch.">compare</a>
              
            </li>
          </ul>
        </li>
      
      
      <li class="tags inertial-hover">tags
        <ul>
          <li class="filter">
            <input type="text" class="dropdown-filter" placeholder="Filter tags" autosave="tags-dropdown-49295"/>
          </li>
          <li class="comprev"><a href="/sinbad/ogre/src/f038953bf2e9">tip</a>
            
              <a rel="nofollow" class='menu-compare'
                 href="/sinbad/ogre/compare/..tip"
                 title="Show changes between tip and the main branch.">compare</a>
            </li><li class="comprev"><a href="/sinbad/ogre/src/41fc194167fa">v1-8-0</a>
            
              <a rel="nofollow" class='menu-compare'
                 href="/sinbad/ogre/compare/..v1-8-0"
                 title="Show changes between v1-8-0 and the main branch.">compare</a>
            </li><li class="comprev"><a href="/sinbad/ogre/src/3467d92e149c">v1-7-4</a>
            
              <a rel="nofollow" class='menu-compare'
                 href="/sinbad/ogre/compare/..v1-7-4"
                 title="Show changes between v1-7-4 and the main branch.">compare</a>
            </li><li class="comprev"><a href="/sinbad/ogre/src/123466e19055">v1-8-0RC1</a>
            
              <a rel="nofollow" class='menu-compare'
                 href="/sinbad/ogre/compare/..v1-8-0RC1"
                 title="Show changes between v1-8-0RC1 and the main branch.">compare</a>
            </li><li class="comprev"><a href="/sinbad/ogre/src/548833a16388">v1-7-3</a>
            
              <a rel="nofollow" class='menu-compare'
                 href="/sinbad/ogre/compare/..v1-7-3"
                 title="Show changes between v1-7-3 and the main branch.">compare</a>
            </li><li class="comprev"><a href="/sinbad/ogre/src/3e9ed7c712e1">v1-7-2</a>
            
              <a rel="nofollow" class='menu-compare'
                 href="/sinbad/ogre/compare/..v1-7-2"
                 title="Show changes between v1-7-2 and the main branch.">compare</a>
            </li><li class="comprev"><a href="/sinbad/ogre/src/c974a42f23b7">v1-7-1</a>
            
              <a rel="nofollow" class='menu-compare'
                 href="/sinbad/ogre/compare/..v1-7-1"
                 title="Show changes between v1-7-1 and the main branch.">compare</a>
            </li><li class="comprev"><a href="/sinbad/ogre/src/8a0b0a2dd186">v1-7-0</a>
            
              <a rel="nofollow" class='menu-compare'
                 href="/sinbad/ogre/compare/..v1-7-0"
                 title="Show changes between v1-7-0 and the main branch.">compare</a>
            </li><li class="comprev"><a href="/sinbad/ogre/src/838cd136e9a8">v1-7-0RC1</a>
            
              <a rel="nofollow" class='menu-compare'
                 href="/sinbad/ogre/compare/..v1-7-0RC1"
                 title="Show changes between v1-7-0RC1 and the main branch.">compare</a>
            </li><li class="comprev"><a href="/sinbad/ogre/src/765cad89d2da">v1-6-5</a>
            
              <a rel="nofollow" class='menu-compare'
                 href="/sinbad/ogre/compare/..v1-6-5"
                 title="Show changes between v1-6-5 and the main branch.">compare</a>
            </li><li class="comprev"><a href="/sinbad/ogre/src/b71cc9d1e5d5">v1-6-4</a>
            
              <a rel="nofollow" class='menu-compare'
                 href="/sinbad/ogre/compare/..v1-6-4"
                 title="Show changes between v1-6-4 and the main branch.">compare</a>
            </li><li class="comprev"><a href="/sinbad/ogre/src/b03c79440d98">v1-6-3</a>
            
              <a rel="nofollow" class='menu-compare'
                 href="/sinbad/ogre/compare/..v1-6-3"
                 title="Show changes between v1-6-3 and the main branch.">compare</a>
            </li><li class="comprev"><a href="/sinbad/ogre/src/236f571e6a06">v1-6-2</a>
            
              <a rel="nofollow" class='menu-compare'
                 href="/sinbad/ogre/compare/..v1-6-2"
                 title="Show changes between v1-6-2 and the main branch.">compare</a>
            </li><li class="comprev"><a href="/sinbad/ogre/src/4f5af67ecf94">v1-6-1</a>
            
              <a rel="nofollow" class='menu-compare'
                 href="/sinbad/ogre/compare/..v1-6-1"
                 title="Show changes between v1-6-1 and the main branch.">compare</a>
            </li><li class="comprev"><a href="/sinbad/ogre/src/fd437752d2f0">v1-6-0</a>
            
              <a rel="nofollow" class='menu-compare'
                 href="/sinbad/ogre/compare/..v1-6-0"
                 title="Show changes between v1-6-0 and the main branch.">compare</a>
            </li><li class="comprev"><a href="/sinbad/ogre/src/82a219c68703">v1-6-0RC1</a>
            
              <a rel="nofollow" class='menu-compare'
                 href="/sinbad/ogre/compare/..v1-6-0RC1"
                 title="Show changes between v1-6-0RC1 and the main branch.">compare</a>
            </li>
        </ul>
      </li>
     
     
      
    </ul>
  
  </div>




<div class="repo-menu" id="repo-desc">
    <ul id="repo-menu-links-mini">
      

      
      <li>
        <a href="/sinbad/ogre/rss" class="rss" title="RSS feed for ogre"></a>
      </li>

      <li><a href="/sinbad/ogre/fork" class="fork" title="Fork"></a></li>
      
        
          <li><a href="/sinbad/ogre/hack" class="patch-queue" title="Patch queue"></a></li>
        
      
      <li>
        <a rel="nofollow" href="/sinbad/ogre/follow" class="follow">follow</a>
      </li>
      
          
      
      
        <li>
          <a class="source" title="Get source"></a>
          <ul class="downloads">
            
              
              <li><a rel="nofollow" href="/sinbad/ogre/get/c28c82519c70.zip">zip</a></li>
              <li><a rel="nofollow" href="/sinbad/ogre/get/c28c82519c70.tar.gz">gz</a></li>
              <li><a rel="nofollow" href="/sinbad/ogre/get/c28c82519c70.tar.bz2">bz2</a></li>
            
          </ul>
        </li>
      
    </ul>

    <h3 id="repo-heading" class="public hg">
      <a class="owner-username" href="/sinbad">sinbad</a> /
      <a class="repo-name" href="/sinbad/ogre">ogre</a>
    
      <span><a href="http://www.ogre3d.org/">http://ogre3d.org/</a></span>
    

    
    </h3>

    
      <p class="repo-desc-description">Official Mercurial repository for OGRE</p>
    

  <div id="repo-desc-cloneinfo">Clone this repository (size: 123.9 MB):
    <a href="https://bitbucket.org/sinbad/ogre" class="https">HTTPS</a> /
    <a href="ssh://hg@bitbucket.org/sinbad/ogre" class="ssh">SSH</a>
    <div id="sourcetree-clone-link" class="clone-in-client mac anonymous help-activated"
         data-desktop-clone-url-ssh="ssh://hg@bitbucket.org/sinbad/ogre"
         data-desktop-clone-url-https="https://bitbucket.org/sinbad/ogre">
        /
      <a class="desktop-ssh"
         href="sourcetree://cloneRepo/ssh://hg@bitbucket.org/sinbad/ogre">SourceTree</a>
      <a class="desktop-https"
         href="sourcetree://cloneRepo/https://bitbucket.org/sinbad/ogre">SourceTree</a>
    </div>
    
    <pre id="clone-url-https">hg clone https://bitbucket.org/sinbad/ogre</pre>
    <pre id="clone-url-ssh">hg clone ssh://hg@bitbucket.org/sinbad/ogre</pre>
    
      <img src="https://bitbucket-assetroot.s3.amazonaws.com/c/photos/2010/Nov/22/c_avatar.bmp" class="repo-avatar" />
    
  </div>

        <a href="#" id="toggle-repo-content"></a>

        

        
          
        

</div>






      
  <div id="source-container">
    

  <div id="source-path">
    <h1>
      <a href="/sinbad/ogre/src" class="src-pjax">ogre</a> /

  
    
      <a href="/sinbad/ogre/src/c28c82519c70/CMake/" class="src-pjax">CMake</a> /
    
  

  
    
      <a href="/sinbad/ogre/src/c28c82519c70/CMake/Packages/" class="src-pjax">Packages</a> /
    
  

  
    
      <span>FindWix.cmake</span>
    
  

    </h1>
  </div>

  <div class="labels labels-csv">
  
    <dl>
  
    
  
  
    
  
  
    <dt>Branch</dt>
    
      
        <dd class="branch unabridged"><a href="/sinbad/ogre/changesets/tip/branch(%22default%22)" title="default">default</a></dd>
      
    
  
</dl>

  
  </div>


  
  <div id="source-view">
    <div class="header">
      <ul class="metadata">
        <li><code>c28c82519c70</code></li>
        
          
            <li>32 loc</li>
          
        
        <li>1012 bytes</li>
      </ul>
      <ul class="source-view-links">
        
        
        <li><a id="embed-link" href="https://bitbucket.org/sinbad/ogre/src/c28c82519c70/CMake/Packages/FindWix.cmake?embed=t">embed</a></li>
        
        <li><a href="/sinbad/ogre/history/CMake/Packages/FindWix.cmake">history</a></li>
        
        <li><a href="/sinbad/ogre/annotate/c28c82519c70/CMake/Packages/FindWix.cmake">annotate</a></li>
        
        <li><a href="/sinbad/ogre/raw/c28c82519c70/CMake/Packages/FindWix.cmake">raw</a></li>
        <li>
          <form action="/sinbad/ogre/diff/CMake/Packages/FindWix.cmake" class="source-view-form">
          
            <input type="hidden" name="diff2" value="63abcdff4877" />
            <select name="diff1">
            
              
                <option value="63abcdff4877">63abcdff4877</option>
              
            
            </select>
            <input type="submit" value="diff" />
          
          </form>
        </li>
      </ul>
    </div>
  
    
      
        <div>
          <table class="highlighttable"><tr><td class="linenos"><div class="linenodiv"><pre><a href="#cl-1"> 1</a>
<a href="#cl-2"> 2</a>
<a href="#cl-3"> 3</a>
<a href="#cl-4"> 4</a>
<a href="#cl-5"> 5</a>
<a href="#cl-6"> 6</a>
<a href="#cl-7"> 7</a>
<a href="#cl-8"> 8</a>
<a href="#cl-9"> 9</a>
<a href="#cl-10">10</a>
<a href="#cl-11">11</a>
<a href="#cl-12">12</a>
<a href="#cl-13">13</a>
<a href="#cl-14">14</a>
<a href="#cl-15">15</a>
<a href="#cl-16">16</a>
<a href="#cl-17">17</a>
<a href="#cl-18">18</a>
<a href="#cl-19">19</a>
<a href="#cl-20">20</a>
<a href="#cl-21">21</a>
<a href="#cl-22">22</a>
<a href="#cl-23">23</a>
<a href="#cl-24">24</a>
<a href="#cl-25">25</a>
<a href="#cl-26">26</a>
<a href="#cl-27">27</a>
<a href="#cl-28">28</a>
<a href="#cl-29">29</a>
<a href="#cl-30">30</a>
<a href="#cl-31">31</a>
<a href="#cl-32">32</a>
</pre></div></td><td class="code"><div class="highlight"><pre><a name="cl-1"></a><span class="c">#-------------------------------------------------------------------</span>
<a name="cl-2"></a><span class="c"># This file is part of the CMake build system for OGRE</span>
<a name="cl-3"></a><span class="c">#     (Object-oriented Graphics Rendering Engine)</span>
<a name="cl-4"></a><span class="c"># For the latest info, see http://www.ogre3d.org/</span>
<a name="cl-5"></a><span class="err">#</span>
<a name="cl-6"></a><span class="c"># The contents of this file are placed in the public domain. Feel</span>
<a name="cl-7"></a><span class="c"># free to make use of it in any way you like.</span>
<a name="cl-8"></a><span class="c">#-------------------------------------------------------------------</span>
<a name="cl-9"></a>
<a name="cl-10"></a><span class="c"># - Try to find Wix</span>
<a name="cl-11"></a><span class="c"># You can help this by defining WIX_HOME in the environment / CMake</span>
<a name="cl-12"></a><span class="c"># Once done, this will define</span>
<a name="cl-13"></a><span class="err">#</span>
<a name="cl-14"></a><span class="c">#  Wix_FOUND - system has Wix</span>
<a name="cl-15"></a><span class="c">#  Wix_BINARY_DIR - location of the Wix binaries</span>
<a name="cl-16"></a>
<a name="cl-17"></a><span class="nb">include</span><span class="p">(</span><span class="s">FindPkgMacros</span><span class="p">)</span>
<a name="cl-18"></a>
<a name="cl-19"></a><span class="c"># Get path, convert backslashes as ${ENV_${var}}</span>
<a name="cl-20"></a><span class="nb">getenv_path</span><span class="p">(</span><span class="s">WIX_HOME</span><span class="p">)</span>
<a name="cl-21"></a>
<a name="cl-22"></a><span class="c"># construct search paths</span>
<a name="cl-23"></a><span class="nb">set</span><span class="p">(</span><span class="s">WIX_PREFIX_PATH</span> <span class="o">${</span><span class="nv">WIX_HOME</span><span class="o">}</span> <span class="o">${</span><span class="nv">ENV_WIX_HOME</span><span class="o">}</span>
<a name="cl-24"></a>        <span class="s2">&quot;C:/Program Files/Windows Installer XML Toolset 3.0&quot;</span>
<a name="cl-25"></a><span class="p">)</span>
<a name="cl-26"></a><span class="nb">find_path</span><span class="p">(</span><span class="s">Wix_BINARY_DIR</span> <span class="s">NAMES</span> <span class="s">candle.exe</span> <span class="s">HINTS</span> <span class="o">${</span><span class="nv">WIX_PREFIX_PATH</span><span class="o">}</span> <span class="s">PATH_SUFFIXES</span> <span class="s">bin</span><span class="p">)</span>
<a name="cl-27"></a>
<a name="cl-28"></a><span class="nb">if</span><span class="p">(</span><span class="s">Wix_BINARY_DIR</span><span class="p">)</span>
<a name="cl-29"></a>        <span class="nb">set</span> <span class="p">(</span><span class="s">Wix_FOUND</span> <span class="s">TRUE</span><span class="p">)</span>
<a name="cl-30"></a><span class="nb">endif</span><span class="p">()</span>
<a name="cl-31"></a>
<a name="cl-32"></a><span class="nb">mark_as_advanced</span><span class="p">(</span><span class="s">Wix_BINARY_DIR</span> <span class="s">Wix_FOUND</span><span class="p">)</span>
</pre></div>
</td></tr></table>
        </div>
      
    
  
  </div>
  


  <div id="mask"><div></div></div>

  </div>

      </div>
    </div>

  </div>

  <div id="footer">
    <ul id="footer-nav">
      <li>Copyright © 2012 <a href="http://atlassian.com">Atlassian</a></li>
      <li><a href="http://www.atlassian.com/hosted/terms.jsp">Terms of Service</a></li>
      <li><a href="http://www.atlassian.com/about/privacy.jsp">Privacy</a></li>
      <li><a href="//bitbucket.org/site/master/issues/new">Report a Bug to Bitbucket</a></li>
      <li><a href="http://confluence.atlassian.com/x/IYBGDQ">API</a></li>
      <li><a href="http://status.bitbucket.org/">Server Status</a></li>
    </ul>
    <ul id="social-nav">
      <li class="blog"><a href="http://blog.bitbucket.org">Bitbucket Blog</a></li>
      <li class="twitter"><a href="http://www.twitter.com/bitbucket">Twitter</a></li>
    </ul>
    <h5>We run</h5>
    <ul id="technologies">
      <li><a href="http://www.djangoproject.com/">Django 1.3.1</a></li>
      <li><a href="//bitbucket.org/jespern/django-piston/">Piston 0.3dev</a></li>
      <li><a href="http://git-scm.com/">Git 1.7.10.3</a></li>
      <li><a href="http://www.selenic.com/mercurial/">Hg 2.2.2</a></li>
      <li><a href="http://www.python.org">Python 2.7.3</a></li>
      <li>15916f97a1bf | bitbucket16</li>
      
    </ul>
  </div>

  <script src="https://dwz7u9t8u8usb.cloudfront.net/m/fbe390655bc5/js/old/global.js"></script>






  <script>
    BB.gaqPush(['_trackPageview']);
  
    /* User specified tracking. */
    BB.gaqPush(
        ['repo._setAccount', 'UA-4018191-4'],
        ['repo._trackPageview']
    );
  
    BB.gaqPush(['atl._trackPageview']);

    

    

    (function () {
        var ga = document.createElement('script');
        ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
        ga.setAttribute('async', 'true');
        document.documentElement.firstChild.appendChild(ga);
    }());
  </script>

</body>
</html>
