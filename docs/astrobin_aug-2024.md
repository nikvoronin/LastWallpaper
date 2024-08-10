# AstroBin IOTD

- [AstroBin IOTD](https://www.astrobin.com/iotd/archive/). Home of astrophotography. AstroBin is an image hosting platform, social network, and marketplace for astrophotographers.
- [AstroBin's read-only API](https://www.astrobin.com/help/api/).

AstroBin implements a basic set of RESTful APIs. AstroBin's APIs currently are limited to retrieving basic information and perform simple searches on images. The supported response types are XML and JSON.

## Image-Of-The-Day Archive Page

> https://www.astrobin.com/iotd/archive/

Contains several `figure` elements which contains image info.

```html
...
    <figure>
        <a href="/pcdsqs/">
            <img src="https://cdn.astrobin.com/thumbs/rS59lKRrZJEs_1000x380_Yd0PTaOB.jpg"
                data-preloaded="true" data-loaded="true" class="astrobin-image" width="1000" height="380"
                data-user-id="4135" data-id="943875" data-id-or-hash="pcdsqs" data-alias="iotd" data-revision="0"
                data-get-thumb-url="None" data-hires-loaded="false"
                alt="NGC 3521 - the Bubble Galaxy, Herbert Walter" />
        </a>
    </figure>
</div>
<div class="data hidden-phone">
    <h3>NGC 3521 - the Bubble Galaxy</h3>
    <p>
        08/09/2024,
        <a class="astrobin-username  astrobin-premium-member" title="Herbert Walter (Herbert_W): Premium member"
            href="/users/Herbert_W/">
            Herbert Walter
        </a>
    </p>
```

### Full image URL

> https://www.astrobin.com/full{FIGURE_HREF}0/

The key for a page with full image is `<figure><a href="`. Let's `FIGURE_HREF` = /pcdsqs/

So the result url is: https://www.astrobin.com/full/pcdsqs/0/

### Preview image + Title + Author

```html
<figure>
    <a href="/pcdsqs/">
        <img
            src="https://cdn.astrobin.com/thumbs/rS59lKRrZJEs_1000x380_Yd0PTaOB.jpg"
            alt="NGC 3521 - the Bubble Galaxy, Herbert Walter"
```

- `src` is an url for small image.
- `alt` attribute contains complex `TITLE, AUTHOR`. Where comma used as a delimiter but this is a weak condition.

### Image title

```html
<div class="data hidden-phone"><h3>THE TITLE</h3>
```

### Publication date

```html
<div class="data hidden-phone">
    ...
    <p>
        08/09/2024,
```

### Author

```html
<div class="data hidden-phone">
    ...
    <p>
        08/09/2024,
        <a class="astrobin-username  ">AUTHOR NAME</a>
```

## Full Image Page

> https://www.astrobin.com/full/pcdsqs/0/

```html
<figure>
    <img src="https://cdn.astrobin.com/thumbs/rS59lKRrZJEs_2560x0_esdlMP5Y.jpg"
```

The first `figure` element contains `img` element with `src` attribute that leads us to the downloadable full size image.

### Video of the day

Sometime there are no image but video:

```html
<figure>
    <video class="video...
```
