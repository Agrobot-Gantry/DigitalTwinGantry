#!/usr/bin/env python

import rospy
from std_msgs.msg import String

def keyboardCommandInput():
	publisher = rospy.Publisher('gantry', String, queue_size=10)
	rospy.init_node('keyboard_command_input', anonymous=True)
	rate = rospy.Rate(10)
	while not rospy.is_shutdown():
		str = raw_input("enter a command: ")
		rospy.loginfo(str)
		publisher.publish(str)
		rate.sleep()

if __name__ == '__main__':
	try:
		keyboardCommandInput()
	except rospy.ROSInterruptException:
		pass