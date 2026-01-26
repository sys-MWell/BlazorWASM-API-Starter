CREATE TABLE [dbo].[ItemTypes] (
    [ItemTypeID]      INT           IDENTITY (1, 1) NOT NULL,
    [TypeName]        VARCHAR (100) NOT NULL,
    [TypeDescription] VARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([ItemTypeID] ASC),
    UNIQUE NONCLUSTERED ([TypeName] ASC)
);

