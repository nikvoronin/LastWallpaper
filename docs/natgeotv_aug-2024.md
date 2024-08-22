# NatGeoTV (Canada)

Each day National Geographic brings you an amazing picture from around the globe.

[NatGeoTV POD](https://www.natgeotv.com/ca/photo-of-the-day). Photo of the day - National Geographic Channel - Canada.

It is also has [RSS](https://feeds.feedburner.com/natgeotv/ca/featured/POD) channel but it is not usefull for our purposes.

## Photo-Of-The-Day Page

> https://www.natgeotv.com/ca/photo-of-the-day

Contains several `li` elements, about 30 by the number of days in a week, which contains url to the image and some description about it. See next example:

```html
<li class="PODItem">
    <a class="DisplayBlock" id="POD22">
        <img style="margin: 0 auto;display:block;" width="940" 
            src="https://assets-natgeotv.fnghub.com/POD/15483.jpg" 
            alt="Aerospace engineer Sophie Harker looks at Apollo CGI. This is from Disaster Autopsy. [Photo of the day - August 2024]" />
    </a>
    <div class="FloatLeft" style="width: 940px;">
        <span class="ItemDate">22 AUGUST 2024</span>
        <span class="FloatRight VMargin5">
            ...
        </span>
    </div>
    <div class="ItemDescription">Aerospace engineer Sophie Harker looks at Apollo CGI. This is from Disaster Autopsy.</div>
    <div class="ItemPhotographer">Blink Films UK</div>
</li>
```

We are interested in the elements with class name equal to `PODItem` - `<li class="PODItem">`. The POD items are listed in reverse chronological order - from new to old.

Anchor `a` of class `DisplayBlock` has `id` with prefix `POD` and two digits of the day number `22`. So finally it is appear as `POD22` or `POD01` for the first day of month (with leading zero).

### Image URL

All of images has the same size 1920x1080 px i.e. FullHD. Image jpeg's names are just a sequential grow ids. So those are not connected in any way with date or other criteria.

Inside a `li` element the only image element `img` contains attribute `src` with the url to the FullHD photo of the day. We ignore content of `alt` attribute because it does not have delimiters.

```html
<li class="PODItem">
    <a class="DisplayBlock" id="POD22">
        <img src="https://assets-natgeotv.fnghub.com/POD/15483.jpg"
             alt="Aerospace engineer Sophie Harker looks at Apollo CGI. This is from Disaster Autopsy. [Photo of the day - August 2024]" />
```

### Publication date

```html
<li class="PODItem">
    ...
    <div>
        <span class="ItemDate">22 AUGUST 2024</span>
```

### Description

```html
<li class="PODItem">
    ...
    <div class="ItemDescription">Aerospace engineer Sophie Harker looks at Apollo CGI. This is from Disaster Autopsy.</div>
```

Usually a description ends with the last sentence: `. This is from {FILM_NAME}.` or `. (This is from {FILM_NAME}.`

### Author

```html
<li class="PODItem">
    ...
    <div class="ItemPhotographer">Blink Films UK</div>
```
