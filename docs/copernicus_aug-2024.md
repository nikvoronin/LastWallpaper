# Copernicus

Copernicus is the Earth observation component of the European Union’s Space programme, looking at our planet and its environment to benefit all European citizens. It offers information services that draw from satellite Earth Observation and in-situ (non-space) data.

[Image of the day](https://www.copernicus.eu/en/media/image-day). This page will give you access to the daily updated gallery containing the newest Copernicus Sentinel images.

## Image of the day page

> https://www.copernicus.eu/en/media/image-day

Contains several `article` elements in a chronological order (from today to the past). Each element contains all data about an image of the day. See next example (a little mess in the text formatting was preserved):

```html
<article role="article" class="p-1 pt-2 pb-2 ">
    <div class="search-results-item-details">
        <div>
            <h4>
                <a href="/en/media/image-day-gallery/severe-floods-pakistan" hreflang="en">Severe floods in Pakistan   </a>
            </h4>
            <div class="mb-2 search-date">
                Date: 
                <time datetime="2024-08-22T12:00:00Z">22/08/2024</time>
            </div>
        </div>
        <div>                <div class="search-tag-btn">
                Land
            </div>                                <div class="search-tag-btn">
                Floods</div><div class="search-tag-btn">Severe Storms
            </div>            </div>
    </div>
    <div class="search-results-item-cover">
          <img src="/system/files/styles/image_of_the_day_square/private/2024-08/image_day/20240822_FloodsInPakistan.png?itok=oZXyhqML" width="800" height="800" alt="Severe floods in Pakistan   " loading="lazy" />
    </div>
</article>
```

### URL and Title of The Image

⚠ Be aware that the original image is very large in size - about tens megabytes (30-40Mb). The format of the image is PNG.

Inside of an `article` element, there is the only element `img` which contains `src` attribute. This attribute contains a part of a full image url. The `alt` attribute can be used as a source of the image title.

```html
<article role="article">
    ...
    <img 
        src="/system/files/styles/image_of_the_day_square/private/2024-08/image_day/20240822_FloodsInPakistan.png?itok=oZXyhqML" 
        alt="Severe floods in Pakistan   " />
```

> src = /system/files/styles/image_of_the_day_square/private  {/2024-08/image_day/20240822_FloodsInPakistan.png}  ?itok=oZXyhqML

https://www.copernicus.eu/system/files/ + {/yyyy-MM/image_day/yyyyMMdd_IMAGENAME.png}

So the final url of the original image is: https://www.copernicus.eu/system/files/2024-08/image_day/20240822_FloodsInPakistan.png

### Publication date

```html
<article role="article">
    ...
    <time datetime="2024-08-22T12:00:00Z">22/08/2024</time>
```

### Description or Title

Is the same as in `img : alt`. See [URL and Title of The Image](#url-and-title-of-the-image) and note a text formatting.

```html
<article role="article">
    ...
    <h4>
        <a href="..." hreflang="en">Severe floods in Pakistan   </a>
    </h4>
```

### Author

Is always the `www.copernicus.eu`.
