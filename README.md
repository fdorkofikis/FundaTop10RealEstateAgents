# Top 10 Real Estate Agents
Assessment for Funda

## Requirements
Determine which makelaar's in Amsterdam have the most object listed for sale. 
Make a table of the top 10. 
Then do the same thing but only for objects with a tuin which are listed for sale.

## Prerequisites
- .NET 9.0
- SQL Server

# How to run
1. The Scalar package has been added for a UI of the exposed OpenApi and can be reached from baseUrl/Scalar

# ChatGpt
- The solution was written with Rider, with the copilot enabled (GPT-4.1).
Did ask copilot to write the tests for me, but most were missing the point and were re-written with one exception that has been commented on code.
That said, the integration is seamless, so have some iuncertainty wetehr autocomplete used was from intelisense or copilot integration.
(example the the Linq used to order, select top 10 makelaars and map them, was autocompleted and it is a little too smart for intellisense)
