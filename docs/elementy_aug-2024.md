# Elementy Science POTD

- [Elementy RSS Feed](https://elementy.ru/rss/kartinka_dnya)

```xml
<rss version="2.0">
    <channel>
        <title>"Элементы": картинки дня</title>
        <link>https://elementy.ru/</link>
        <description/>
        <lastBuildDate>06 Aug 2024 11:01:00 +0300</lastBuildDate>
        <image>
            <title>"Элементы": картинки дня</title>
            <url>http://elementy.ru/images/eltdesign/title.gif</url>
            <link>https://elementy.ru/</link>
        </image>
        <item>
            <enclosure type="image/jpeg" url="https://elementy.ru/images/kartinka_dnya/potd_diodon_1_703.jpeg"/>
            <category>Ихтиология</category>
            <description/>
            <link>https://elementy.ru/kartinka_dnya/1921/Kolyuchiy_shar</link>
            <pubDate>05 Aug 2024 05:33:29 +0300</pubDate>
            <title>Колючий шар</title>
        </item>
        <item>
        ...
```

## Channel Item

```xml
<item>
    <enclosure 
        type="image/jpeg" 
        url="https://elementy.ru/images/kartinka_dnya/potd_diodon_1_703.jpeg"/>
    <category>Ихтиология</category>
    <description/>
    <link>https://elementy.ru/kartinka_dnya/1921/Kolyuchiy_shar</link>
    <pubDate>05 Aug 2024 05:33:29 +0300</pubDate>
    <title>Колючий шар</title>
</item>
```

- __title__ - is a name of picture.
- __category__ - category of science.
- __enclosure__
    - __type__ - attached media type.
    - __url__ - url to image preview.
- __link__ - article link.
- __pubDate__ - the date-time of image publication.

Url of the original image contain in `eclosure::url` attribute. Just remove image resolution marker `_703` coming before a file extension. See next example:

```text
item : enclosure : url
https://elementy.ru/images/kartinka_dnya/potd_diodon_1_703.jpeg
https://elementy.ru/images/kartinka_dnya/potd_diodon_1.jpeg
```
