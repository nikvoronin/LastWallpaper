module Types

    type ImageOfTheDay = {
        FileName: string
        Title: string Option
        Description: string Option
        Copyright: string Option
        }
    
    type Msg<'a> =
    | UpdateNow of 'a
