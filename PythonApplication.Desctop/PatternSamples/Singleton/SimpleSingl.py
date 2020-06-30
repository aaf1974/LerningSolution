

class SimpleSingl(object):
    __age = 50


    def __new__(cls):
        if not hasattr(cls, 'instance'):
            cls.instance = super(SimpleSingl, cls).__new__(cls)
        return cls.instance


    @property
    def age(self):
        return self.__age
 
    @age.setter
    def age(self, age):
        self.__age = age

