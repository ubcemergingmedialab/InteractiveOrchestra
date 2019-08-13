import requests


if __name__ == "__main__":
    
    Content = open("ConductorSamples_OcculusTrial_Test.xml").read()
    From xml.etree import XML
    Etree = XML(Content)
    Print Etree.text, Etree.value, Etree.getchildren()



