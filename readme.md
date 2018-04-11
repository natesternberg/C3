# Credit card categorizer (C3)

Since 2004, I've maintained a somewhat anal-retentive habit: reviewing every purchase I make and categorizing it (for instance, "groceries", "clothes", "transportation", etc.)  I'd paste credit card bills into Excel and walk through each line item manually.  The idea was that I could, with some tedious Excel formulas, calculate how my, for instance, transportation expenses changed month-over-month.  Manually labelling each charge got old fast, especially since my purchasing habits are pretty repetitive.  It seemed silly to have to label "Trader Joe's" as "groceries" every month.

A naive approach to automating this would be some sort of rule-based system, e.g., a hard-coded map of "Trader Joe's" to "groceries".  But what you actually get on a credit card bill contains additional terms that might vary by location (for instance, "Trader Joe's #00001370 Seattle Wa").  I wanted to capture the rule that if you see "Trader Joe's" anywhere, just ignore whatever comes next, and call it "groceries."

So I wrote a simple (NaiveBayes) supervised machine-learning classifier.  Fortunately, I had plenty of training data from the years I'd been categorizing by hand.  This worked surprisingly well (NaiveBayes is neat that way, you get a lot of bang for your algorithmic complexity buck), and eliminated most of the categorization work.  But it does makes mistakes, both because the algorithm is not especially sophisticated, and because some purchase descriptions are naturally ambiguous (a purchase from Amazon could be nearly any category, so the input signal is noisy).  So I added a GUI (.NET WPF) interface with an Excel-like grid to display its predictions and allow me to fix any mistakes.

![](https://raw.githubusercontent.com/natesternberg/C3/master/images/screenshot1.PNG)

While I was at it, I also had it calculate a simple pivot table-ish report to help me explore trends.

![](https://raw.githubusercontent.com/natesternberg/C3/master/images/screenshot2.PNG)

![](https://raw.githubusercontent.com/natesternberg/C3/master/images/screenshot3.PNG)

I named the overall project "C3", short for "credit card categorizer"...because I'm bad at names.  Note to self: call my next app "Project Ganymede" or something catchy.