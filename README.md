# ExhallCCStatisticsCreator
Here is a C# console app that I will use to read data from an excel file that contains statistics for every season since 1994. This includes seasona an career stats for three teams, awards at club and league level and milestones and much more! The tricky part will be automating the process as the excel files contains many sources of data, not just a single tab. 

So what I have done here is create an app that does the following:
1. Reads the data from one tab of the excel file
2. Filters out unuseful rows
3. Parses the data to a class where we can perform any actions on the data accordingly
4. Then we create an INSERT statements for my POstgreSQL database.
5. Then all the data is then written to a txt file that we can run and create the data!

Watch this space as I will add further functionality in.
