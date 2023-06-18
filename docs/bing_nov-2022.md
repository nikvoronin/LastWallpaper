# Bing Image of The Day

## HPImages API

### Request

> https://bingwallpaper.microsoft.com/api/BWC/getHPImages?screenWidth=3840&screenHeight=2160

```text
https://bingwallpaper.microsoft.com/api/BWC/getHPImages
    ? screenWidth  ={0}
    & screenHeight ={1}
```

### Response

As a JSON with a description of 8 last images. See next example

```json
{
    "images": [{
        "startdate": "20221121",
        "fullstartdate": "202211210800",
        "enddate": "20221122",
        "url": "https://bingwallpaperimages.azureedge.net/hpimages/Latest/3840x2160/20221121.jpg",
        "urlbase": "https://www.bing.com/th?id=OHR.FIFA2022_EN-US9006895256_UHD.jpg&rf=LaDigue_UHD.jpg&pid=hp&w=3840&h=2160&rs=1&c=4",
        "copyright": "Ahmad Bin Ali Stadium in Doha, Qatar (© Qatar 2022/Supreme Committee via Getty Images)",
        "copyrightlink": "https://www.bing.com/search?q=2022+fifa+world+cup&form=hpcapt&filters=HpDate%3a%2220221121_0800%22",
        "copyrighttext": "© Qatar 2022/Supreme Committee via Getty Images",
        "title": "Ahmad Bin Ali Stadium in Doha, Qatar",
        "quiz": "/search?q=Bing+homepage+quiz&filters=WQOskey:%22HPQuiz_20221121_FIFA2022%22&FORM=HPQUIZ",
        "wp": false,
        "hsh": "efcce1a99b80512f44c833b269e1e603",
        "drk": 1,
        "top": 1,
        "bot": 1,
        "hs": []
    }, // { ... }
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

- `startdate` - publish date.
- `url` - image with Bing logo.
- `urlbase` - image without Bing logo.
- `copyrighttext` - copyrights of the image.
- `title` - picture description. What is on the picture.
