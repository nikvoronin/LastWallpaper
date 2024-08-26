# NASA Image of the Day

[www.nasa.gov](https://www.nasa.gov/image-of-the-day/). NASA Image of the Day.

## Image of the day page

> https://www.nasa.gov/image-of-the-day/

Contains multiple `a` elements with `hds-gallery-item-link` and `hds-gallery-image-link` class name in a chronological order (from today to the past). Each element contains url of the full sized image, brief image description and link to the page with image details. See example below:

```html
<a class="hds-gallery-item-link hds-gallery-image-link"
    href="https://www.nasa.gov/image-detail/electrified-powertrain-flight-demonstration/">
    <div class="hds-gallery-item-overlay hds-gallery-image-overlay">
        <div class="hds-gallery-item-caption hds-gallery-image-caption">The Dash 7 that will be modified into a hybrid electric research vehicle under NASA’s Electrified Powertrain Flight Demonstration (EPFD)...
        </div>
    </div>
    <img src="https://www.nasa.gov/wp-content/uploads/2024/08/lrc-2024-h1-p-epfd-0711-photo-credit-nasa-david-c-bowman.jpg"
            alt="A four engine turboprop aircraft wrapped in a red and white livery with logos and names of each partner on the project sits under the lights inside an aircraft hangar. On the ground in front of the plane is an electric powertrain with an electric motor and battery pack that will soon be swapped out with one of the aircraft’s traditional engines to form a hybrid electric system."
            loading="lazy" />
</a>
```

### Image title

```html
<a class="hds-gallery-item-link hds-gallery-image-link" ...>
    <div ...>
        <div class="hds-gallery-item-caption hds-gallery-image-caption">The Dash 7 that will be modified into a hybrid electric research vehicle under NASA’s Electrified Powertrain Flight Demonstration (EPFD)...
        </div>
```

An innerText of the `div` element with `hds-gallery-item-caption` and `hds-gallery-image-caption` classes.

### Image URL and scene description

```html
<a class="hds-gallery-item-link hds-gallery-image-link" ...>
    <img src="https://www.nasa.gov/wp-content/uploads/2024/08/lrc-2024-h1-p-epfd-0711-photo-credit-nasa-david-c-bowman.jpg"
            alt="Description of the scene in the picture" />
```

An only `img` element where `src` attrubute leads to the image in full size and `alt` contains a description of the scene in the picture.

### Publication date

There is no publication date, so we just get the latest image and set the date as today.

### Image credits

```html
<a class="hds-gallery-item-link hds-gallery-image-link"
    href="https://www.nasa.gov/image-detail/electrified-powertrain-flight-demonstration/">
```

The `href` attribute contains url leading to the page with image details.

> href = https://www.nasa.gov/image-detail/electrified-powertrain-flight-demonstration/

#### Detailed description of the image

We are interesting in `div` with `hds-attachment-single__credit` class name.

```html
<div class="hds-attachment-single__credit color-carbon-30 padding-0 p-sm">Image Credit: NASA/David C. Bowman</div>
```

An innerText of the element contains prefix `Image Credit:` and credits/author `NASA/David C. Bowman`.
