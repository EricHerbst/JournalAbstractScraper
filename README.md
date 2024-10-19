**The Brief**:
- My wife was studying for the American Board of Veterinary Practitioners Canine and Feline Specialist exam, which required reading ALL article abstracts from several peer reviewed journals over the last 5 years. 
- Thus she required hunting down several thousand abstracts for this distinction, and with two young kids, the time to just collect all the required information felt overwhelming and reduced her available study time.
- Being a loving husband I told her I'd create a small program to collect those for her.
- Having a background myself in academia (PhD & MSc in brain and muscle metabolism, yeah, big jump, right?), I already knew the important information to collect and where to get it.

**Execution**:
This is a simple C# console app that:
- Takes in a small set of search instructions
    - (This is hard coded in a method for now, nothing fancy, but could be done via UI quite simply if desired without changing much)
- Assembles a search string url based on the search criteria
- Parses the total page count of the results based on the intial search
- Assembles all html string payloads from the results asynchronously
- Parses all pages into a list of Journal Article results
- Saves them to a .csv file

**Alternatives**:
- PubMed offers several simple APIs for retrieving journal article DOIs (digital object identifiers) as well advanced APIs great for genome nucleotide sequence blasting, but nothing (that I could find) that solved our problem of simply retrieving all the required abstracts for reading.
- PubMed was a more centralized source for retrieving all abstracts as opposed to creating individual parsers for each journal. The AJAX calls from several of the journals I spiked the concept with were also causing issues. However, the application implements interfaces where needed in case more fetchers and parsers are needed.

**Remarks:**
- The project is kept small and clean. Interfaces were used where more than one solution / approach could be desired
- Console updates are used in several places to demonstrate the app is still operating
