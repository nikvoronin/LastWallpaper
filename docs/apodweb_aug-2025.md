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

Second paragraph `p` contains publication date in the format of `yyyy MMMM d`. See following HTML example

```html
<center>
...
<p>
...
<p>

2025 July 26
<br> 
```

### URL of the large image

```html
<center>
    <a href="image/2508/DoubleClusterBrechersmall.jpg" ...>
```

HREF attribute format must follow this pattern: `image/{year+month}/{image-filename}.jpg`. See HTML example above.

Breakdown:

- Starts with: `image/`
- Followed by `year+month` in `YYMM` format (e.g., August 2025 → 2508). Month must be two digits (e.g., 08 for August)
- Ends with a valid .jpg|.png|.jpeg filename extension.

### Image title

Second `center` element contains image title

```html
<center>
<b> The Double Cluster in Perseus </b> <br> 
...
```

### Image credits and/or authors

Credits and/or author names are also present in the second `center` element.

```html
<center>
...

<b>Image Credit &
<a href="lib/about_apod.html#srapply">Copyright</a>: </b>

<a href="https://astrodoc.ca/about-me/">Ron Brecher</a>

</center>
```

Credits is a whole trimmed text after `:` column sign. See following examples

```plain

 Dawn of the Crab   

Image and Text Credit:

Bradley E. Schaefer


```

And more complex

```html
<center>
<b> Collision at Asteroid Dimorphos </b> <br> 
<b> Video Credit: </b> 
<a href="https://www.asi.it/">ASI</a>, 
<a href="https://www.nasa.gov/">NASA</a>,
<a href="https://dart.jhuapl.edu/">Johns Hopkins APL</a>,
<a href="https://www.nasa.gov/planetarydefense/dart/dart-news">DART</a>,
<a href="https://www.asi.it/en/planets-stars-universe/solar-system-and-beyond/liciacube/">LICIACube</a>, 
<a href="https://nssdc.gsfc.nasa.gov/nmc/spacecraft/display.action?id=2021-110C">LUKE</a>, 
<a href="https://www.iop.org/">IOP</a>
</center>
```

Produced but not trimmed yet `innerText`

```plain
 Collision at Asteroid Dimorphos   
 Video Credit:  
ASI, 
NASA,
Johns Hopkins APL,
DART,
LICIACube, 
LUKE, 
IOP
```
