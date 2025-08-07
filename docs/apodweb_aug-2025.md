# NASA Astronomy Picture of the Day

[Astronomy Picture of the Day](https://apod.nasa.gov/apod/). Today's Astronomy Picture of the Day.

## Image of the day page

> https://apod.nasa.gov/apod/astropix.html

Contains two `center` elements and picture explanation. See example below:

```html
<center>
...
<p>

2025 August 7
<br> 
<a href="image/2508/DoubleClusterBrechersmall.jpg">
...</a>
</center>

<center>
<b> The Double Cluster in Perseus </b> <br> 

<b>Image Credit &
<a href="lib/about_apod.html#srapply">Copyright</a>: </b>

<a href="https://astrodoc.ca/about-me/">Ron Brecher</a>

</center> <p> 
<b> Explanation: </b> 
...
```

### Publication date

TODO?

### URL of the large image

```html
<center>
    <a href="image/2508/DoubleClusterBrechersmall.jpg" ...>
```

HREF attribute format must follow this pattern: `image/{year+month}/{image-filename}.jpg`. See HTML example above.

Breakdown:

- Starts with: `image/`
- Followed by `year+month` in `YYMM` format (e.g., August 2025 → 2508). Month must be two digits (e.g., 08 for August)
- Ends with a valid .jpg filename. Only `.jpg` extensions are supported.

### Image title

Second `center` element contains image title and credits

```html
<center>
<b> The Double Cluster in Perseus </b> <br> 
...
```

### Image credits

```html
<center>
...

<b>Image Credit &
<a href="lib/about_apod.html#srapply">Copyright</a>: </b>

<a href="https://astrodoc.ca/about-me/">Ron Brecher</a>

</center>
```

Whole text after `:` ..................
