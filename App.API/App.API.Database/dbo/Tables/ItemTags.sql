CREATE TABLE [dbo].[ItemTags] (
    [ItemID] INT NOT NULL,
    [TagID]  INT NOT NULL,
    PRIMARY KEY CLUSTERED ([ItemID] ASC, [TagID] ASC),
    CONSTRAINT [fk_itemtags_itemid] FOREIGN KEY ([ItemID]) REFERENCES [dbo].[Items] ([ItemID]),
    CONSTRAINT [fk_itemtags_tagid] FOREIGN KEY ([TagID]) REFERENCES [dbo].[Tags] ([TagID])
);

