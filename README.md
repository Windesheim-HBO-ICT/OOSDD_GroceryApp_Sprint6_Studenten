#GroceryApp sprint7 Studentversie  

## branche strategie
- main is voor de (werkende) releases
- dev is waar features worden samen gevoegd en er intergratie testen kunnen worden gedaan voor release
- feature/* is een branche voor het uitwerken van 1 usecase/userstory
- hotfix/* is voor kleine bug fixes die gelijk naar main word doorgevoerd
- release/* is voor het maken en behouden van releases

## Studentenversie:
 
UC17 Boodschappenlijst in database is compleet uitgewerkt.  

UC18 BoodschappenlijstItems in database.  
- Gebruik het voorbeeld van UC17 om zelf de GroceryListItemsRepository te wijzigen zodat boodschappenlijstitems uit de database komen.  

UC19 Product in database en nieuw product aanmaken --> zelfstandig uitwerken.  
- Volg UC17 om producten uit de database te kunnen halen.  
- De Add() functie in ProductService moet uitgewerkt zijn om nieuwe producten te kunnen aanmaken.  
- Maak een NewProductViewModel om het aanmaken van nieuwe producten te ondersteunen. Alleen gebruikers met de admin Role mogen nieuwe producten aanmaken.  
- Maak een NewProductView voor het invoerscherm.  
- Voeg een ToolbarItemn toe aan de ProductView, zodat vanuit dit scherm nieuwe producten kunnen worden aangemaakt.  
- Zorg ervoor dat als er een nieuw product is aangemaakt, deze meteen zichtbaar is in de Productlijst van de ProductView.  
- Denk aan de registratie van de View, ViewModel en registreren van de route naar NewProductView.  

UC20 Categorieën in database.  
- Gebruik het voorbeeld van UC17 om zelf de Categorieën te wijzigen zodat boodschappenlijstitems uit de database komen.

UC21 ProductCategorieën in database.  
- Gebruik het voorbeeld van UC17 om zelf de ProductCategorieën te wijzigen zodat boodschappenlijstitems uit de database komen.

UC22 Client in database.  
- Gebruik het voorbeeld van UC17 om zelf de Client te wijzigen zodat boodschappenlijstitems uit de database komen.
