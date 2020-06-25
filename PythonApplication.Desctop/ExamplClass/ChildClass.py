from ExampleBase import ExampleBase

class ExampleChild(ExampleBase):
    childId = 666

    def __init__(self, baseId, childId):
        ExampleBase.__init__(self, baseId)
        self.childId = childId


    def display_info(self):
        print("/n")
        print("Info from child class")
        ExampleBase.display_info(self)
        print("ChildId:", self.childId)


