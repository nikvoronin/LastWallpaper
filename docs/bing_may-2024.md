# Bing Image of The Day. May 2024

## HPImageArchive

### Request

> https://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=10

```text
https://www.bing.com/HPImageArchive.aspx
    ? format = js
    & idx    = 0
    & n      = 10
    & mkt    = en-US
```

- __format__: js, xml, rss - format of the list of images.
- __idx__ - index of the image (zero based: 0, 1, 2, ...)
- __n__ - how many images u want to (integer > 0, 8 at most).
- _mkt_ - location (en-US, en-GB, ...). Does not matter.

### Response

As a JSON with a description of 8 last images. See next example

```json
{
    "images": [
        {
            "startdate": "20240529",
            "fullstartdate": "202405290700",
            "enddate": "20240530",
            "url": "/th?id=OHR.MullOtter_ROW8840823080_1920x1080.jpg&rf=LaDigue_1920x1080.jpg&pid=hp",
            "urlbase": "/th?id=OHR.MullOtter_ROW8840823080",
            "copyright": "Eurasian otters, Loch Spelve, Isle of Mull, Scotland (© Neil Henderson/Alamy)",
            "copyrightlink": "https://www.bing.com/search?q=Eurasian+otter&form=hpcapt",
            "title": "Info",
            "quiz": "/search?q=Bing+homepage+quiz&filters=WQOskey:%22HPQuiz_20240529_MullOtter%22&FORM=HPQUIZ",
            "wp": true,
            "hsh": "f716bb42354f383c582340b35e1080f3",
            "drk": 1,
            "top": 1,
            "bot": 1,
            "hs": []
        },
        // ...
    ],
    "tooltips": {
        "loading": "Loading...",
        "previous": "Previous image",
        "next": "Next image",
        "walle": "This image is not available to download as wallpaper.",
        "walls": "Download this image. Use of this image is restricted to wallpaper only."
    }
}
```

Valuable fields:

- __startdate__ - publish date.
- __urlbase__ - relatve url to the image.
- __copyright__ - copyrights of the image.
- __title__ - description of the picture. What's happening in the picture.

### URL of the image

```json
"urlbase": "/th?id=OHR.MullOtter_ROW8840823080",
```

> image link = `https://www.bing.com` + `{urlbase}` + `{resolution}`

Resolutions:

- HD: `_1280x720.jpg`
- FullHD: `_1920x1080.jpg`
- UltraHD (4K): `_UHD.jpg`

Example: `https://www.bing.com/th?id=OHR.MullOtter_ROW8840823080_UHD.jpg`

## References

- [Is there a way to get Bing's photo of the day?](https://stackoverflow.com/questions/10639914/is-there-a-way-to-get-bings-photo-of-the-day) // StackOverflow
