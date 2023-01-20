# Compile and execute the web application.
# Chris Joakim, Microsoft, November 2022

echo "clean..."
gradle clean

echo "build..."
gradle build

echo "starting web app..."
gradle bootrun > out\webapp.txt
