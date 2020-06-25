class ExampleBase(object):
    baseId = 123


    def __init__(self, id):
        self.baseId = id

    def display_info(self):
        print("Base class info")
        print("BaseId:", self.baseId)


    def __str__(self):
        return "override toString() {}".format(type(self))