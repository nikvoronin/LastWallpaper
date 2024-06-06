# Wikipedia POTD

- [Wikipedia POTD](https://en.wikipedia.org/wiki/Wikipedia:Picture_of_the_day). About wikipedia, the picture of the day.
- [MediaWiki. API:Picture of the day viewer](https://www.mediawiki.org/wiki/API:Picture_of_the_day_viewer). In this tutorial, you will learn how to build a Wikipedia:Picture of the day viewer using the MediaWiki Action API and Python.

Images can have arbitrary proportions. Both portrait and album and square. So for wallpapers is appropriate images with album proportions only. POTD can be video. Be aware of `File:{}.jpg` extension.

## POTD Filename

```text
https://en.wikipedia.org/w/api.php
    ? action        = query
    & format        = json
    & formatversion = 2
    & prop          = images
    & titles        = Template:POTD
    / 2024-06-06
```

GET request returns JSON below:

```json
{
    "batchcomplete": true,
    "query": {
        "pages": [
            {
                "pageid": 77085108,
                "ns": 10,
                "title": "Template:POTD/2024-06-06",
                "images": [
                    {
                        "ns": 6,
                        "title": "File:Martial eagle (Polemaetus bellicosus).jpg"
                    }
                ]
            }
        ]
    }
}
```

Multiple filenames are possible:

```json
{
    "batchcomplete": true,
    "query": {
        "pages": [
            {
                "pageid": 75711497,
                "ns": 10,
                "title": "Template:POTD/2024-01-14",
                "images": [
                    {
                        "ns": 6,
                        "title": "File:NNC-US-1849-G$1-Liberty head (Ty1).jpg"
                    },
                    {
                        "ns": 6,
                        "title": "File:NNC-US-1854-G$1-Indian head (Ty2).jpg"
                    },
                    {
                        "ns": 6,
                        "title": "File:NNC-US-1856-G$1-Indian head (Ty3).jpg"
                    }
                ]
            }
        ]
    }
}
```

## Full Image URL

> titles = File:{url-encoded-filename.jpg}

```text
https://en.wikipedia.org/w/api.php
    ? action        = query
    & format        = json
    & formatversion = 2
    & prop          = imageinfo
    & iiprop        = url
    & titles        = File:Martial%20eagle%20(Polemaetus%20bellicosus).jpg
```

Response JSON:

> query → pages[0] → imageinfo[0] → url

```json
{
    ...
    "query": {
        "pages": [
            {
                "pageid": 69915226,
                "ns": 6,
                "title": "File:Martial eagle (Polemaetus bellicosus).jpg",
                "imagerepository": "shared",
                "imageinfo": [
                    {
                        "url": "https://upload.wikimedia.org/wikipedia/commons/6/61/Martial_eagle_%28Polemaetus_bellicosus%29.jpg",
                        "descriptionurl": "https://commons.wikimedia.org/wiki/File:Martial_eagle_(Polemaetus_bellicosus).jpg",
                        "descriptionshorturl": "https://commons.wikimedia.org/w/index.php?curid=67819541"
                    }
                ]
            }
        ]
    }
}
```

## Title and Credits

```text
http://en.wikipedia.org/w/api.php
    ? action        = query
    & format        = json
    & formatversion = 2
    & prop          = imageinfo
    & iiprop        = extmetadata
    & titles        = File:Martial%20eagle%20(Polemaetus%20bellicosus).jpg
```

- imageinfo
    - extmetadata
        - ImageDescription → value
        - Credit → value
        - Artist → value

All `values` could contain HTML formated content.

Response JSON:

```json
{
    ...
    "query": {
        "pages": [
            {
                "pageid": 69915226,
                "ns": 6,
                "title": "File:Martial eagle (Polemaetus bellicosus).jpg",
                "imagerepository": "shared",
                "imageinfo": [
                    {
                        "extmetadata": {
                            ...
                            "ImageDescription": {
                                "value": "Martial eagle (<i>Polemaetus bellicosus</i>), Matetsi Safari Area, Zimbabwe",
                                "source": "commons-desc-page"
                            },
                            ...
                            "Credit": {
                                "value": "<span class=\"int-own-work\" lang=\"en\">Own work</span>, from <a rel=\"nofollow\" class=\"external text\" href=\"https://www.sharpphotography.co.uk/\">Sharp Photography, sharpphotography.co.uk</a>",
                                "source": "commons-desc-page"
                            },
                            "Artist": {
                                "value": "<bdi><a href=\"https://www.wikidata.org/wiki/Q54800218\" class=\"extiw\" title=\"d:Q54800218\"><span title=\"Scottish wildlife photographer\">Charles J. Sharp</span></a>\n</bdi>",
                                "source": "commons-desc-page"
                            },
                            ...
                            "Copyrighted": {
                                "value": "True",
                                "source": "commons-desc-page",
                                "hidden": ""
                            },
                            ...
                        }
                    }
                ]
            }
        ]
    }
}
```
