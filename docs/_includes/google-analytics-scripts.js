/******************************************************************************/

$("window").ready(
  function () {
    configureGaReleaseLinks();
    configureGaInstallerLinks();
  }
);

/******************************************************************************/

function configureGaReleaseLinks() {

  $(".link-release").click(

    function ( event ) {

      let LinkClicked = this;
      let TagHref = $(LinkClicked).attr("href");
      let Tag = TagHref.split('/').pop().split('#')[0].split('?')[0];

      ga(
        'send',
        {
          hitType: 'event',
          eventCategory: 'downloads',
          eventAction: 'link-release',
          eventLabel: Tag,
          eventValue: 0,
          transport: 'beacon'
        }
      );

    }

  );

  console.log("Done: configureGaReleaseLinks");

}

/******************************************************************************/

function configureGaInstallerLinks() {

  $(".link-installer").click(

    function ( event ) {

      let LinkClicked = this;
      let TagHref = $(LinkClicked).attr("href");
      let Tag = TagHref.split('/').pop().split('#')[0].split('?')[0];

      ga(
        'send',
        {
          hitType: 'event',
          eventCategory: 'downloads',
          eventAction: 'link-installer',
          eventLabel: Tag,
          eventValue: 0,
          transport: 'beacon'
        }
      );

    }

  );

  console.log("Done: configureGaInstallerLinks");

}

/******************************************************************************/